using MediatR;
using Authorization.Application.Services.Abstractions;
using Authorization.Domain.Abstractions;
using Authorization.Infrastructure.Repositories;
using Authorization.Application.Services.Jwt;

namespace Authorization.WebApi.DI
{
    public static class ApplicationServicesRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
            services.AddScoped<IJwtService, JwtService>();
        }
    }
}
