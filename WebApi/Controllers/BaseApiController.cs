using Microsoft.AspNetCore.Mvc;
using Domain.Common;

namespace WebApi.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return StatusCode(result.Error!.StatusCode, new 
        { 
            code = result.Error.Message 
        });
    }
}