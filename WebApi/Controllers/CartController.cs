using Domain.Constants;
using Domain.DTOs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize(Policy = PolicyNames.BuyerGroup)]
[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService _cartService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CartItemDTO dto)
        => HandleResult(await _cartService.AddAsync(dto, User));

    [HttpPut]
    public async Task<IActionResult> UpdateQuantity([FromBody] CartItemDTO dto)
        => HandleResult(await _cartService.UpdateQuantityAsync(dto, User));

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove([FromRoute] Guid productId)
        => HandleResult(await _cartService.RemoveAsync(productId, User));

    [HttpDelete("all")]
    public async Task<IActionResult> RemoveAll()
        => HandleResult(await _cartService.RemoveAllAsync(User));

    [HttpGet("total-data-details")]
    public async Task<IActionResult> GetTotalNum()
        => HandleResult(await _cartService.GetTotalNumAsync(User));

    [HttpGet("items")]
    public async Task<IActionResult> GetAllItems()
        => HandleResult(await _cartService.GetAllItemsAsync(User));
}
