using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class ReadChatConsumer : IConsumer<ReadChatRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public ReadChatConsumer(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<ReadChatRequest> context)
        {
            var request = context.Message;
            await _hubContext.Clients.User(request.UserId.ToString())
                .ReadChat(request.ChatRoomId);
        }
    }
}
