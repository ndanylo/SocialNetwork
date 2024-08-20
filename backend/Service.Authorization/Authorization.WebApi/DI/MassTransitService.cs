using Authorization.Application.Consumers;
using MassTransit;

namespace Authorization.WebApi.DI
{
    public static class MassTransitService
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<RegisterUserConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQSection = configuration.GetSection("RabbitMQ");

                    cfg.Host(rabbitMQSection["Host"] ?? throw new ArgumentNullException("RabbitMQ host is not configured."), "/", h =>
                    {
                        h.Username(rabbitMQSection["Username"] ?? throw new ArgumentNullException("RabbitMQ Username is not configured."));
                        h.Password(rabbitMQSection["Password"] ?? throw new ArgumentNullException("RabbitMQ Password is not configured."));
                    });

                    cfg.ReceiveEndpoint("register-user", ep =>
                    {
                        ep.ConfigureConsumer<RegisterUserConsumer>(context);
                    });
                });
            });

        }
    }
}
