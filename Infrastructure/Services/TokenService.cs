using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Common;
using Domain.Constants;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService(DataContext _context, IConfiguration _configuration) : ITokenService
{
    public TokenDto GenerateTokens(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");
        var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            }),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            Expires = expiry,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(accessTokenDescriptor));
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = expiry
        };
    }

    public async Task<Result<Guid>> GetUserIdAsync(ClaimsPrincipal userClaims)
    {
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Result<Guid>.Failure(ErrorMessages.User_Not_Found);

        var userRole = userClaims.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(userRole))
            return Result<Guid>.Failure(ErrorMessages.Unauthorized);

        var user = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == Guid.Parse(userId))
            .Select(x => new { 
                x.Id, 
                x.RefreshToken, 
                x.RefreshTokenExpiry 
            })
            .FirstOrDefaultAsync();
        if (user == null)
            return Result<Guid>.Failure(ErrorMessages.User_Not_Found);
        if (user.RefreshToken == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<Guid>.Failure(ErrorMessages.Invalid_Refresh_Token);

        return Result<Guid>.Success(user.Id);
    }
}