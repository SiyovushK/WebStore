using System.Security.Claims;
using Domain.Common;
using Domain.DTOs;

namespace Infrastructure.Interfaces;

public interface IAccountService
{
    Task<Result<TokenDto>> RegisterAsync(RegisterDto registerDto);
    Task<Result<TokenDto>> LoginAsync(LoginDto loginDto);
    Task LogoutAsync(string refreshToken);
    Task<Result<GetUserDto>> GetSelfAsync(ClaimsPrincipal userClaims);
    Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken);
}
