using Domain.Constants;
using Domain.DTOs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService _productService) : BaseApiController
{
    [Authorize(Policy = PolicyNames.SellerOnly)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        => HandleResult(await _productService.CreateAsync(dto, User));

    [Authorize(Policy = PolicyNames.SellerOnly)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        => HandleResult(await _productService.UpdateAsync(id, dto, User));

    [Authorize(Policy = PolicyNames.SellerOnly)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromBody] Guid id)
        => HandleResult(await _productService.DeleteAsync(id, User));

    [Authorize(Policy = PolicyNames.BuyerGroup)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
        => HandleResult(await _productService.GetByIdAsync(id, User));

    [Authorize(Policy = PolicyNames.BuyerGroup)]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterDto filter)
        => HandleResult(await _productService.GetAllAsync(filter, User));
}
