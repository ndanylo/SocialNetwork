using MassTransit;
using MessageBus.Contracts.Requests;

namespace Chats.WebApi.DI
{
    public static class MassTransitService
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddMassTransit(x =>
            {
                x.AddRequestClient<ReadChatRequest>();
                x.AddRequestClient<SendMessageRequest>();
                x.AddRequestClient<SetMessageReadRequest>();

                x.AddRequestClient<GetUserByIdRequest>();
                x.AddRequestClient<GetUserListByIdRequest>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQSection = configuration.GetSection("RabbitMQ");

                    cfg.Host(rabbitMQSection["Host"] ?? throw new ArgumentNullException("RabbitMQ host is not configured."), "/", h =>
                    {
                        h.Username(rabbitMQSection["Username"] ?? throw new ArgumentNullException("RabbitMQ Username is not configured."));
                        h.Password(rabbitMQSection["Password"] ?? throw new ArgumentNullException("RabbitMQ Password is not configured."));
                    });
                });
            });
        }
    }
}
