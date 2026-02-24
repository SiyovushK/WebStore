using Infrastructure.AutoMapper;
using Infrastructure.DI;
using WebApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Domain.DTOs;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddApiControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<InfrastructureProfile>());
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
