using Domain.Constants;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
using AutoMapper;
using System.Security.Claims;

namespace Infrastructure.Services;

public class AccountService(DataContext _context, ITokenService _tokenService, IMapper _mapper) : IAccountService
{
    public async Task<Result<TokenDto>> RegisterAsync(RegisterDto registerDto)
    {
        var emailExists = await _context.Users.AnyAsync(x => x.Email == registerDto.Email);
        if (emailExists)
            return Result<TokenDto>.Failure(ErrorMessages.Email_Already_Exists);

        var user = _mapper.Map<User>(registerDto);
        user.Email = registerDto.Email.ToLower();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var response = await CreateUserResponse(user);

        return Result<TokenDto>.Success(response);
    }

    public async Task<Result<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return Result<TokenDto>.Failure(ErrorMessages.Invalid_Credentials);

        var response = await CreateUserResponse(user);
        return Result<TokenDto>.Success(response);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        if (user != null && user.RefreshTokenExpiry > DateTime.UtcNow)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Result<GetUserDto>> GetSelfAsync(ClaimsPrincipal userClaims)
    {
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Result<GetUserDto>.Failure(ErrorMessages.User_Not_Found);

        var user = await _context.Users.FindAsync(Guid.Parse(userId));
        if (user == null)
            return Result<GetUserDto>.Failure(ErrorMessages.User_Not_Found);
        if (user.RefreshToken == null ||user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<GetUserDto>.Failure(ErrorMessages.Invalid_Refresh_Token);

        var userDto = _mapper.Map<GetUserDto>(user);
        return Result<GetUserDto>.Success(userDto);
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<TokenDto>.Failure(ErrorMessages.Invalid_Refresh_Token);

        var response = await CreateUserResponse(user);
        return Result<TokenDto>.Success(response);
    }

    private async Task<TokenDto> CreateUserResponse(User user)
    {
        var tokenDto = _tokenService.GenerateTokens(user);

        user.RefreshToken = tokenDto.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        
        await _context.SaveChangesAsync();

        return tokenDto;
    }
}
