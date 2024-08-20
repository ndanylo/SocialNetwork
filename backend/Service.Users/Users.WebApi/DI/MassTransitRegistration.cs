using MassTransit;
using MessageBus.Contracts.Requests;
using Users.Application.Consumers;

namespace Users.WebApi.DI
{
    public static class MassTransitRegistration
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddRequestClient<RegisterUserRequest>();

                x.AddRequestClient<IsPostLikedByUserRequest>();
                x.AddRequestClient<GetUserPostsRequest>();

                x.AddRequestClient<FriendRemovedRequest>();
                x.AddRequestClient<FriendRequestAcceptedRequest>();
                x.AddRequestClient<FriendRequestCancelledRequest>();
                x.AddRequestClient<FriendRequestDeclinedRequest>();
                x.AddRequestClient<FriendRequestReceivedRequest>();

                x.AddConsumer<GetUserFriendsConsumer>();
                x.AddConsumer<GetUserListByIdConsumer>();
                x.AddConsumer<GetUserByIdConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQSection = configuration.GetSection("RabbitMQ");

                    cfg.Host(rabbitMQSection["Host"] ?? throw new ArgumentNullException("RabbitMQ host is not configured."), "/", h =>
                    {
                        h.Username(rabbitMQSection["Username"] ?? throw new ArgumentNullException("RabbitMQ Username is not configured."));
                        h.Password(rabbitMQSection["Password"] ?? throw new ArgumentNullException("RabbitMQ Password is not configured."));
                    });

                    cfg.ReceiveEndpoint("get-user-friends", ep =>
                    {
                        ep.ConfigureConsumer<GetUserFriendsConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("get-user-list-by-id", ep =>
                    {
                        ep.ConfigureConsumer<GetUserListByIdConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("get-user-by-id", ep =>
                    {
                        ep.ConfigureConsumer<GetUserByIdConsumer>(context);
                    });

                    cfg.UseJsonSerializer();
                });
            });
        }
    }
}
