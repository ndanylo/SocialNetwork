using Microsoft.EntityFrameworkCore;
using Authorization.Infrastructure.EF;

namespace Authorization.WebApi.DI
{
    public static class DbContextRegistration
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserCredentialsDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
