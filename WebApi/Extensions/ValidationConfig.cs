using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extensions;

public static class WebApiRegistration
{
    public static IMvcBuilder AddApiControllers(this IServiceCollection services)
    {
        return services.AddControllers()
            .ConfigureApiBehaviorOptions(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    var error = context.ModelState.Values
                        .SelectMany(x => x.Errors)
                        .FirstOrDefault();

                    var response = new
                    {
                        code = error?.ErrorMessage ?? "Validation_Error",
                        message = "Validation failed"
                    };

                    return new BadRequestObjectResult(response);
                };
            })
            .AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }
}