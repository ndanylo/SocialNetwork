using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class FriendRequestDeclinedConsumer : IConsumer<FriendRequestDeclinedRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public FriendRequestDeclinedConsumer(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<FriendRequestDeclinedRequest> context)
        {
            var request = context.Message;
            await _hubContext.Clients.User(request.SenderId.ToString())
                .FriendRequestDeclined(request.ReceiverId);
        }
    }
}
