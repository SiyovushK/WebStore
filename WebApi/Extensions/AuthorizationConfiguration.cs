using Domain.Constants;

namespace WebApi.Extensions;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.SellerGroup, policy =>
                policy.RequireRole(Roles.Seller, Roles.Admin, Roles.SuperAdmin));

            options.AddPolicy(PolicyNames.AdminGroup, policy =>
                policy.RequireRole(Roles.Admin, Roles.SuperAdmin));

            options.AddPolicy(PolicyNames.BuyerGroup, policy =>
                policy.RequireAuthenticatedUser()); // valid toke must be provided, but no specific role is required

            options.AddPolicy(PolicyNames.SellerOnly, policy =>
                policy.RequireRole(Roles.Seller));
        });

        return services;
    }
}