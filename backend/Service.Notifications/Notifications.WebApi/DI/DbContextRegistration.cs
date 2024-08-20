using Microsoft.EntityFrameworkCore;
using Notifications.Infrastructure.EF;

namespace Notifications.WebApi.DI
{
    public static class DbContextRegistration
    {
        public static void AddDbContextServices(this IServiceCollection services)
        {
            services.AddDbContext<NotificationDbContext>(options =>
                options.UseSqlite("Filename=notificationDb.db"));
        }
    }
}
