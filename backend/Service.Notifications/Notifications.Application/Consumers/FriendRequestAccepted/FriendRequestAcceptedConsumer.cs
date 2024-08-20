using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class FriendRequestAcceptedConsumer : IConsumer<FriendRequestAcceptedRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public FriendRequestAcceptedConsumer(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<FriendRequestAcceptedRequest> context)
        {
            var request = context.Message;
            await _hubContext.Clients.User(request.SenderId.ToString())
                .FriendRequestAccepted(request.ReceiverId);
        }
    }
}
