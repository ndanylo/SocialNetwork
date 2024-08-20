using Authorization.Domain.Entities;
using Authorization.Domain.ValueObjects;
using Authorization.Infrastructure.EF;
using Microsoft.AspNetCore.Identity;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<UserCredentials, IdentityRole<UserId>>(options =>
        {
            options.User.AllowedUserNameCharacters = "";
        })
        .AddEntityFrameworkStores<UserCredentialsDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
