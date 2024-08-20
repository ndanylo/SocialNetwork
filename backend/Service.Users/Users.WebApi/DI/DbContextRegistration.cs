using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.EF;

namespace Users.WebApi.DI
{
    public static class DbContextRegistration
    {
        public static void AddDbContextServices(this IServiceCollection services)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlite("Filename=notificationDb.db"));
        }
    }
}
