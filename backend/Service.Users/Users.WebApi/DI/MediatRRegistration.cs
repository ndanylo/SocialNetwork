using OnlineChat.Application.FriendRequests.Commands.CancelFriendRequest;
using OnlineChat.Application.FriendRequests.Queries.GetSentFriendRequests;
using Users.Application.Commands.AcceptFriendRequest;
using Users.Application.Commands.CreateUser;
using Users.Application.Commands.DeclineFriendRequest;
using Users.Application.Commands.RemoveFriend;
using Users.Application.Commands.SendFriendRequest;
using Users.Application.Queries.GetAllUsers;
using Users.Application.Queries.GetFriends;
using Users.Application.Queries.GetReceivedFriendRequests;
using Users.Application.Queries.GetUsersDetails;

namespace Users.WebApi.DI
{
    public static class MediatRRegistration
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AcceptFriendRequestCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(CancelFriendRequestCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(DeclineFriendRequestCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(RemoveFriendCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(SendFriendRequestCommand).Assembly);

                cfg.RegisterServicesFromAssembly(typeof(GetAllUsersQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetFriendsQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetReceivedFriendRequestsQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetSentFriendRequestsQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetUsersDetailsQuery).Assembly);
            });

            return services;
        }
    }
}
