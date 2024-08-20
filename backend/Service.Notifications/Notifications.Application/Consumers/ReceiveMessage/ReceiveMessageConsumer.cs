using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class ReceiveMessageConsumer : IConsumer<SendMessageRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public ReceiveMessageConsumer(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<SendMessageRequest> context)
        {
            var message = context.Message;
            await _hubContext.Clients.User(message.ReceiverId.ToString())
                .ReceiveMessage(message.Message);
        }
    }
}
