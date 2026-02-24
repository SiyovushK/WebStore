using System.Security.Claims;
using Domain.Common;
using Domain.DTOs;

namespace Infrastructure.Interfaces;

public interface ICartService
{
    Task<Result<List<GetCartItemDTO>>> GetAllItemsAsync(ClaimsPrincipal userClaims);
    Task<Result<CartItemDetailsDTO>> GetTotalNumAsync(ClaimsPrincipal userClaims);
    Task<Result<bool>> RemoveAsync(Guid productId, ClaimsPrincipal userClaims);
    Task<Result<bool>> RemoveAllAsync(ClaimsPrincipal userClaims);
    Task<Result<bool>> AddAsync(CartItemDTO dto, ClaimsPrincipal userClaims);
    Task<Result<bool>> UpdateQuantityAsync(CartItemDTO dto, ClaimsPrincipal userClaims);
}
