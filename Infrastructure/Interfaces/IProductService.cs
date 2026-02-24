using System.Security.Claims;
using Domain.Common;
using Domain.DTOs;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<Result<List<ProductDto>>> GetAllAsync(ProductFilterDto filter, ClaimsPrincipal userClaims);
    Task<Result<ProductDto>> GetByIdAsync(Guid productId, ClaimsPrincipal userClaims);
    Task<Result<ProductDto>> CreateAsync(CreateProductDto dto, ClaimsPrincipal userClaims);
    Task<Result<ProductDto>> UpdateAsync(Guid productId, UpdateProductDto dto, ClaimsPrincipal userClaims);
    Task<Result<bool>> DeleteAsync(Guid productId, ClaimsPrincipal userClaims);
}
