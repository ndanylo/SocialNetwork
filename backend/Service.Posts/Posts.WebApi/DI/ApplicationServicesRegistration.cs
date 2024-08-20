using Posts.Application.Mappings;
using Posts.Application.Services;
using Posts.Application.Services.Abstractions;
using Posts.Domain.Abstractions;
using Posts.Infrastructure.Repositories;
using Posts.Infrastructure.Services;

namespace Posts.WebApi.DI
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILikeRepository, LikeRepository>();
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
