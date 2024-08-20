using Microsoft.EntityFrameworkCore;
using Posts.Infrastructure.EF;

namespace Posts.WebApi.DI
{
    public static class DbContextRegistration
    {
        public static void AddDbContextServices(this IServiceCollection services)
        {
            services.AddDbContext<PostDbContext>(options =>
                options.UseSqlite("Filename=notificationDb.db"));
        }
    }
}
