using MassTransit;
using MessageBus.Contracts.Requests;
using Posts.Application.Consumers;

namespace Posts.WebApi.DI
{
    public static class MassTransitRegistration
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddMassTransit(x =>
            {
                x.AddRequestClient<CreateNotificationRequest>();
                x.AddRequestClient<GetUserListByIdRequest>();
                x.AddRequestClient<GetUserByIdRequest>();
                x.AddRequestClient<GetUserFriendsRequest>();

                x.AddConsumer<GetUserPostsConsumer>();
                x.AddConsumer<GetPostByIdConsumer>();
                x.AddConsumer<IsPostLikedByUserConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQSection = configuration.GetSection("RabbitMQ");

                    cfg.Host(rabbitMQSection["Host"] ?? throw new ArgumentNullException("RabbitMQ host is not configured."), "/", h =>
                    {
                        h.Username(rabbitMQSection["Username"] ?? throw new ArgumentNullException("RabbitMQ Username is not configured."));
                        h.Password(rabbitMQSection["Password"] ?? throw new ArgumentNullException("RabbitMQ Password is not configured."));
                    });

                    cfg.ReceiveEndpoint("get-user-posts", e =>
                    {
                        e.ConfigureConsumer<GetUserPostsConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("is-post-liked-by-user", e =>
                    {
                        e.ConfigureConsumer<IsPostLikedByUserConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("get-post-by-id", e =>
                    {
                        e.ConfigureConsumer<GetPostByIdConsumer>(context);
                    });
                });
            });
        }
    }
}
