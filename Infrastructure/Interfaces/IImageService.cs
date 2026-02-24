using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces;

public interface IImageService
{
    Task<Result<List<string>>> UploadImagesAsync(Guid productId, List<IFormFile> files);
    Task<Result<bool>> DeleteImageAsync(string imageUrl);
    Task<Result<bool>> DeleteAllProductImagesAsync(Guid productId);
}
