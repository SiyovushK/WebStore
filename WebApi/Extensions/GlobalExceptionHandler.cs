using Microsoft.AspNetCore.Diagnostics;

namespace WebApi.Extensions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            code = "Internal_Server_Error",
            message = "Unhandled server error occurred. Please try again later."
        }, cancellationToken);

        return true;
    }
}