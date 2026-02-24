using System.Security.Claims;
using AutoMapper;
using Domain.Common;
using Domain.Constants;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class CartService(DataContext _context, IMapper _mapper, ITokenService _tokenService) : ICartService
{
    public async Task<Result<bool>> AddAsync(CartItemDTO dto, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == dto.ProductId);
        if (product is null)
            return Result<bool>.Failure(ErrorMessages.Product_Not_Found);
        if (product.StockQuantity < dto.Quantity)
            return Result<bool>.Failure(ErrorMessages.Insufficient_Stock);

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userIdResult.Value && c.ProductId == dto.ProductId);
        if (existingItem != null)
            return Result<bool>.Failure(ErrorMessages.Item_Already_In_Cart);

        var cartItem = _mapper.Map<CartItem>(dto);
        cartItem.UserId = userIdResult.Value;

        await _context.CartItems.AddAsync(cartItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateQuantityAsync(CartItemDTO dto, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.ProductId == dto.ProductId);
        if (cartItem is null)
            return Result<bool>.Failure(ErrorMessages.Item_Not_Found);
        if (cartItem.UserId != userIdResult.Value)
            return Result<bool>.Failure(ErrorMessages.Unauthorized);

        var product = await _context.Products
            .AsNoTracking()
            .Select(p => new { p.Id, p.StockQuantity, p.IsDeleted })
            .FirstOrDefaultAsync(p => p.Id == dto.ProductId);
        if (product is null || product.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.Product_Unavailable);
        if (product.StockQuantity < dto.Quantity)
            return Result<bool>.Failure(ErrorMessages.Insufficient_Stock);

        cartItem.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveAsync(Guid productId, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userIdResult.Value && c.ProductId == productId);

        if (cartItem is null) 
            return Result<bool>.Failure(ErrorMessages.Item_Not_Found);

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveAllAsync(ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var items = await _context.CartItems
            .Where(c => c.UserId == userIdResult.Value)
            .ToListAsync();
            
        if (!items.Any())
            return Result<bool>.Success(true);

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<CartItemDetailsDTO>> GetTotalNumAsync(ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var result = await _context.CartItems
            .AsNoTracking()
            .Where(c => c.UserId == userIdResult.Value)
            .GroupBy(c => c.UserId)
            .Select(g => new 
            {
                TotalSum = g.Sum(c => c.Product!.Price * c.Quantity),
                TotalCount = g.Sum(c => c.Quantity)
            })
            .FirstOrDefaultAsync();

        var details = new CartItemDetailsDTO
        {
            TotalPrice = result?.TotalSum ?? 0,
            TotalCount = result?.TotalCount ?? 0
        };

        return Result<CartItemDetailsDTO>.Success(details);
    }

    public async Task<Result<List<GetCartItemDTO>>> GetAllItemsAsync(ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var cartItems = await _context.CartItems
            .IgnoreQueryFilters() // Ignore global filters to include soft-deleted products
            .Include(c => c.Product)
            .Where(c => c.UserId == userIdResult.Value)
            .ToListAsync();

        if (!cartItems.Any())
            return Result<List<GetCartItemDTO>>.Success(new List<GetCartItemDTO>());

        var result = cartItems.Select(item => 
        {
            var product = item.Product;
            var isDeleted = product!.IsDeleted;
            var isOutOfStock = product.StockQuantity < item.Quantity;

            bool isAvailable = !isDeleted && !isOutOfStock;
            string status = AppMessages.Available;

            if (isDeleted) status = AppMessages.Product_Unavailable;
            else if (isOutOfStock) status = AppMessages.Insufficient_Stock;

            var cartItemDto = _mapper.Map<GetCartItemDTO>(item);
            cartItemDto.IsAvailable = isAvailable;
            cartItemDto.StatusMessage = status;

            return cartItemDto;
        }).ToList();

        return Result<List<GetCartItemDTO>>.Success(result);
    }

}
