using MassTransit;
using MessageBus.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Hubs;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Consumers
{
    public class FriendRemovedConsumer : IConsumer<FriendRemovedRequest>
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;
        private readonly ILogger<FriendRemovedConsumer> _logger;

        public FriendRemovedConsumer(
            IHubContext<ChatHub, IChatHub> hubContext,
            ILogger<FriendRemovedConsumer> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FriendRemovedRequest> context)
        {
            var request = context.Message;

            _logger.LogInformation("Received FriendRemovedRequest: UserId={UserId}, FriendId={FriendId}",
                request.UserId, request.FriendId);

            try
            {
                await _hubContext.Clients.User(request.FriendId.ToString())
                    .FriendRemoved(request.UserId);

                _logger.LogInformation("Successfully notified FriendId={FriendId} about UserId={UserId} removal.",
                    request.FriendId, request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while notifying FriendId={FriendId} about UserId={UserId} removal.",
                    request.FriendId, request.UserId);
            }
        }
    }
}
