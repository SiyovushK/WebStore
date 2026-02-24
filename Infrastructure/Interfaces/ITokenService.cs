using System.Security.Claims;
using Domain.Common;
using Domain.DTOs;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface ITokenService
{
    TokenDto GenerateTokens(User user);
    Task<Result<Guid>> GetUserIdAsync(ClaimsPrincipal userClaims);
}
