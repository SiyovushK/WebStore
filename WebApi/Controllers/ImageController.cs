using System.ComponentModel.DataAnnotations;
using Domain.Constants;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize(Policy = PolicyNames.SellerGroup)]
[ApiController]
[Route("api/[controller]")]
public class ImageController(IImageService _imageService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> UploadImages([Required] Guid productId, [FromForm] List<IFormFile> files)
        => HandleResult(await _imageService.UploadImagesAsync(productId, files));

    [HttpDelete]
    public async Task<IActionResult> DeleteImage([FromBody, Required] string imageUrl)
        => HandleResult(await _imageService.DeleteImageAsync(imageUrl));

    [HttpDelete("all/{productId}")]
    public async Task<IActionResult> DeleteAllProductImages([FromBody, Required] Guid productId)
        => HandleResult(await _imageService.DeleteAllProductImagesAsync(productId));
}
