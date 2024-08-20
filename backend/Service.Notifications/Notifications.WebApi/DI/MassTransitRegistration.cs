using MassTransit;
using MessageBus.Contracts.Requests;
using Notifications.Application.Consumers;

namespace Notifications.WebApi.DI
{
    public static class MassTransitRegistration
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddRequestClient<GetUserByIdRequest>();
                x.AddRequestClient<GetPostByIdRequest>();

                x.AddConsumer<CreateNotificationConsumer>();
                x.AddConsumer<ReceiveMessageConsumer>();
                x.AddConsumer<ReadChatConsumer>();

                x.AddConsumer<FriendRemovedConsumer>();
                x.AddConsumer<FriendRequestReceivedConsumer>();
                x.AddConsumer<FriendRequestAcceptedConsumer>();
                x.AddConsumer<FriendRequestCancelledConsumer>();
                x.AddConsumer<FriendRequestDeclinedConsumer>();
                x.AddConsumer<SetMessageReadConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQSection = configuration.GetSection("RabbitMQ");

                    cfg.Host(rabbitMQSection["Host"] ?? throw new ArgumentNullException("RabbitMQ host is not configured."), "/", h =>
                    {
                        h.Username(rabbitMQSection["Username"] ?? throw new ArgumentNullException("RabbitMQ Username is not configured."));
                        h.Password(rabbitMQSection["Password"] ?? throw new ArgumentNullException("RabbitMQ Password is not configured."));
                    });

                    cfg.ReceiveEndpoint("create-notification", e =>
                    {
                        e.ConfigureConsumer<CreateNotificationConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("receive-message", e =>
                    {
                        e.ConfigureConsumer<ReceiveMessageConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("read-chat-messages", e =>
                    {
                        e.ConfigureConsumer<ReadChatConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("friend-removed", e =>
                    {
                        e.ConfigureConsumer<FriendRemovedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("friend-request-received", e =>
                    {
                        e.ConfigureConsumer<FriendRequestReceivedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("friend-request-accepted", e =>
                    {
                        e.ConfigureConsumer<FriendRequestAcceptedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("friend-request-cancelled", e =>
                    {
                        e.ConfigureConsumer<FriendRequestCancelledConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("friend-request-declined", e =>
                    {
                        e.ConfigureConsumer<FriendRequestDeclinedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("send-message-read-notification", e =>
                    {
                        e.ConfigureConsumer<SetMessageReadConsumer>(context);
                    });

                    cfg.UseJsonSerializer();
                });
            });
        }
    }
}
