using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Domain.Entities;
using Domain.Common;
using Domain.Constants;

namespace Infrastructure.Services;

public class ImageService : IImageService
{
    private readonly Cloudinary _cloudinary;
    private readonly DataContext _context;

    public ImageService(IOptions<CloudinarySettings> config, DataContext context)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true; // always use HTTPS
        _context = context;
    }

    private async Task<Result<bool>> ProductCheck(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return Result<bool>.Failure(ErrorMessages.Product_Not_Found);

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<string>>> UploadImagesAsync(Guid productId, List<IFormFile> files)
    {
        var uploadedUrls = new List<string>();

        var check = await ProductCheck(productId);
        if (!check.IsSuccess)
            return Result<List<string>>.Failure(check.Error);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var file in files)
            {
                if (file.Length == 0) continue;
    
                await using var stream = file.OpenReadStream();
    
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    // organizes images into folders per product in Cloudinary
                    Folder = $"products/{productId}",
                    // auto-optimize format and quality
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto")
                };
    
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    
                if (uploadResult.Error != null)
                    return Result<List<string>>.Failure(ErrorMessages.Internal_Server_Error);
                    // add logging here for uploadResult.Error.Message
    
                var imageStore = new ImageStore
                {
                    Url = uploadResult.SecureUrl.ToString(),
                    ProductId = productId
                };
    
                _context.ImageStores.Add(imageStore);
                uploadedUrls.Add(imageStore.Url);
            }
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            foreach (var publicId in uploadedUrls.Select(ExtractPublicId))
                await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            return Result<List<string>>.Failure(ErrorMessages.Internal_Server_Error);
            // add logging here for the exception
        }
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Result<List<string>>.Success(uploadedUrls);
    }

    public async Task<Result<bool>> DeleteImageAsync(string imageUrl)
    {
        // Extract the public_id from the URL
        // Cloudinary URL format: https://res.cloudinary.com/{cloud}/image/upload/v{version}/{public_id}.{ext}
        var publicId = ExtractPublicId(imageUrl);

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Result != "ok")
            return Result<bool>.Failure(ErrorMessages.Internal_Server_Error);

        var imageStore = await _context.ImageStores
            .FirstOrDefaultAsync(i => i.Url == imageUrl);

        if (imageStore != null)
        {
            _context.ImageStores.Remove(imageStore);
            await _context.SaveChangesAsync();
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAllProductImagesAsync(Guid productId)
    {
        var check = await ProductCheck(productId);
        if (!check.IsSuccess)
            return Result<bool>.Failure(check.Error);

        // Delete the entire folder for this product in Cloudinary
        await _cloudinary.DeleteResourcesByPrefixAsync($"products/{productId}");

        var images = await _context.ImageStores
            .Where(i => i.ProductId == productId)
            .ToListAsync();

        _context.ImageStores.RemoveRange(images);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    // ------------------------------------------------------------------
    // Extracts the Cloudinary public_id from a secure URL
    // e.g. ".../image/upload/v1234567890/products/guid/filename.jpg"
    //   => "products/guid/filename"
    // ------------------------------------------------------------------
    private static string ExtractPublicId(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var segments = uri.AbsolutePath.Split('/');

        // Find the "upload" segment index and take everything after it,
        // skipping the version segment (starts with 'v' + digits)
        var uploadIndex = Array.IndexOf(segments, "upload");
        var relevantSegments = segments
            .Skip(uploadIndex + 1)
            .SkipWhile(s => System.Text.RegularExpressions.Regex.IsMatch(s, @"^v\d+$"))
            .ToArray();

        var publicIdWithExtension = string.Join("/", relevantSegments);

        // Remove file extension
        var lastDot = publicIdWithExtension.LastIndexOf('.');
        return lastDot >= 0
            ? publicIdWithExtension[..lastDot]
            : publicIdWithExtension;
    }
}