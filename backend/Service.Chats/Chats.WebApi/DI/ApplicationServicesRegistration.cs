using System.Reflection;
using Chats.Application.Services.Abstractions;
using Chats.Domain.Abstractions;
using Chats.Infrastructure.Repositories;
using Chats.Infrastructure.Services;
using OnlineChat.Application.Mappings;

namespace Chats.WebApi.DI
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddAutoMapper(cfg =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var logger = serviceProvider.GetRequiredService<ILogger<MappingProfile>>();
                cfg.AddProfile(new MappingProfile(logger));
            }, Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
