using Notifications.Application.Commands.CreateNotification;
using Notifications.Application.Commands.DeleteAllNotifications;
using Notifications.Application.Commands.DeleteNotification;
using Notifications.Application.Queries.GetNotifications;

namespace Notifications.WebApi.DI
{
    public static class MediatRRegistration
    {
        public static void AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetNotificationsQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(DeleteNotificationCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(DeleteAllNotificationsCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(CreateNotificationCommand).Assembly);
            });
        }
    }
}
