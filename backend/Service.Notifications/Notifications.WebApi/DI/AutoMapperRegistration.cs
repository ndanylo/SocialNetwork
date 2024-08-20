using System.Reflection;

namespace Notifications.WebApi.DI
{
    public static class AutoMapperRegistration
    {
        public static void AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, Assembly.GetExecutingAssembly());
        }
    }
}
