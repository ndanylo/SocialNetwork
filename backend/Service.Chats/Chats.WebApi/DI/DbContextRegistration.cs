using Chats.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Chats.WebApi.DI
{
    public static class DbContextRegistration
    {
        public static void AddDbContextServices(this IServiceCollection services)
        {
            services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlite("Filename=chatsDb.db"));
        }
    }
}
