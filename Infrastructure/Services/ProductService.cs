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

public class ProductService(DataContext _context, IMapper _mapper, ITokenService _tokenService) : IProductService
{
    public async Task<Result<ProductDto>> CreateAsync(CreateProductDto dto, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);
        
        Product product = _mapper.Map<Product>(dto);
        product.SellerId = userIdResult.Value;
        
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        ProductDto productDto = _mapper.Map<ProductDto>(product);
        return Result<ProductDto>.Success(productDto);
    }

    public async Task<Result<bool>> DeleteAsync(Guid productId, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);
        
        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.IsDeleted)
            return Result<bool>.Failure(ErrorMessages.Product_Not_Found);
        if (product.SellerId != userIdResult.Value)
            return Result<bool>.Failure(ErrorMessages.Unauthorized);

        product.IsDeleted = true;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<ProductDto>>> GetAllAsync(ProductFilterDto filter, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            string searchLower = filter.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchLower) || 
                                     p.Description.ToLower().Contains(searchLower));
        }

        if (filter.Category.HasValue)
            query = query.Where(p => p.Category == filter.Category.Value);

        if (filter.SellerId.HasValue)
            query = query.Where(p => p.SellerId == filter.SellerId.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        var products = await query
            .AsNoTracking()
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var productDtos = _mapper.Map<List<ProductDto>>(products);
        
        // Marking products in cart and favourites
        if (userIdResult.IsSuccess && products.Any())
        {
            var userId = userIdResult.Value;
            var productIds = products.Select(p => p.Id).ToList();

            var cartProductIds = await _context.CartItems
                .Where(c => c.UserId == userId && productIds.Contains(c.ProductId))
                .Select(c => c.ProductId)
                .ToListAsync();

            var favouriteProductIds = await _context.Favourites
                .Where(f => f.UserId == userId && productIds.Contains(f.ProductId))
                .Select(f => f.ProductId)
                .ToListAsync();

            // Converting lists to HashSet for O(1) lookups
            var cartSet = new HashSet<Guid>(cartProductIds);
            var favSet = new HashSet<Guid>(favouriteProductIds);

            // 4. Iterating through products and marking InCart and InFavourite
            foreach (var dto in productDtos)
            {
                dto.InCart = cartSet.Contains(dto.Id); // if HashSet contains the product ID, mark InCart as true, else false
                dto.InFavourite = favSet.Contains(dto.Id); // same here
            }
        }

        return Result<List<ProductDto>>.Success(productDtos);
    }

    public async Task<Result<ProductDto>> GetByIdAsync(Guid productId, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
            return Result<ProductDto>.Failure(ErrorMessages.Product_Not_Found);

        var inCart = await _context.CartItems
            .AnyAsync(c => c.ProductId == productId && c.UserId == userIdResult.Value);
        var inFavourite = await _context.Favourites
            .AnyAsync(f => f.ProductId == productId && f.UserId == userIdResult.Value);        
        
        ProductDto productDto = _mapper.Map<ProductDto>(product);
        productDto.InCart = inCart;
        productDto.InFavourite = inFavourite;

        return Result<ProductDto>.Success(productDto);
    }

    public async Task<Result<ProductDto>> UpdateAsync(Guid productId, UpdateProductDto dto, ClaimsPrincipal userClaims)
    {
        var userIdResult = await _tokenService.GetUserIdAsync(userClaims);
        
        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.IsDeleted)
            return Result<ProductDto>.Failure(ErrorMessages.Product_Not_Found);
        if (product.SellerId != userIdResult.Value)
            return Result<ProductDto>.Failure(ErrorMessages.Unauthorized);

        _mapper.Map(dto, product);
        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        ProductDto productDto = _mapper.Map<ProductDto>(product);
        return Result<ProductDto>.Success(productDto);
    }
}
