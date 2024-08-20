using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class FriendRequestCancelledConsumer : IConsumer<FriendRequestCancelledRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public FriendRequestCancelledConsumer(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<FriendRequestCancelledRequest> context)
        {
            var request = context.Message;
            await _hubContext.Clients.User(request.ReceiverId.ToString())
                .FriendRequestCancelled(request.SenderId);
        }
    }
}
