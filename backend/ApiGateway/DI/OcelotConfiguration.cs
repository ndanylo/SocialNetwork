using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway.DI
{
    public static class OcelotConfiguration
    {
        public static void ConfigureOcelot(this IServiceCollection services)
        {
            services.AddOcelot();
        }

        public static async Task UseOcelotMiddleware(this WebApplication app)
        {
            await app.UseOcelot();
        }
    }
}