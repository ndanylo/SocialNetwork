using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class SetMessageReadConsumer : IConsumer<SetMessageReadRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public SetMessageReadConsumer(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<SetMessageReadRequest> context)
        {
            var request = context.Message;
            await _hubContext.Clients.User(request.Reader.Id.ToString())
                .SetMessageRead(request.ChatRoomId, request.MessageId, request.Reader);
        }
    }
}
