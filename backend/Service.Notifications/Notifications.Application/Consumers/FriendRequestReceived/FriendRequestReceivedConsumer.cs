using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class FriendRequestReceivedConsumer : IConsumer<FriendRequestReceivedRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public FriendRequestReceivedConsumer(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<FriendRequestReceivedRequest> context)
        {
            var user = context.Message.SenderViewModel;
            var receiverId = context.Message.ReceiverId;

            await _hubContext.Clients.User(receiverId.ToString())
                .FriendRequestReceived(user);
        }
    }
}
