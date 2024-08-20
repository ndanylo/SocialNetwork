using Notifications.Application.Services;
using Notifications.Application.Services.Abstraction;
using Notifications.Domain.Abstractions;
using Notifications.Infrastructure.Repositories;

namespace Notifications.WebApi.DI
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
        }
    }
}
