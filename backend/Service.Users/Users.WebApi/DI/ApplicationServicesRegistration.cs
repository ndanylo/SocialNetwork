using Users.Application.Services;
using Users.Domain.Abstractions;
using OnlineChat.Infrastructure.Repositories;
using Users.Application.Services.Abstractions;
using Chats.Application.Mappings;
using Posts.Application.Services.Abstractions;
using Posts.Application.Services;

namespace Users.WebApi.DI
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IImageService, ImageService>();
            services.AddAutoMapper(cfg =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var imageService = serviceProvider.GetRequiredService<IImageService>();
                cfg.AddProfile(new MappingProfile(imageService));
            });
            return services;
        }
    }
}
