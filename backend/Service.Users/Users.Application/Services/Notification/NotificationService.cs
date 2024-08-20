using CSharpFunctionalExtensions;
using MassTransit;
using Posts.Application.Services.Abstractions;
using MessageBus.Contracts.Requests;
using Users.Application.ViewModels;
using Microsoft.Extensions.Logging;

namespace Posts.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IPublishEndpoint publishEndpoint, ILogger<NotificationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<Result> FriendRemoved(Guid userId, Guid friendId)
        {
            var request = new FriendRemovedRequest
            {
                UserId = userId,
                FriendId = friendId
            };

            _logger.LogInformation("Publishing FriendRemovedRequest: UserId={UserId}, FriendId={FriendId}", request.UserId, request.FriendId);

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }

        public async Task<Result> FriendRequestAccepted(Guid receiverId, Guid senderId)
        {
            var request = new FriendRequestAcceptedRequest
            {
                ReceiverId = receiverId,
                SenderId = senderId
            };

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }

        public async Task<Result> FriendRequestCancelled(Guid receiverId, Guid senderId)
        {
            var request = new FriendRequestCancelledRequest
            {
                ReceiverId = receiverId,
                SenderId = senderId
            };

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }

        public async Task<Result> FriendRequestDeclined(Guid receiverId, Guid senderId)
        {
            var request = new FriendRequestDeclinedRequest
            {
                ReceiverId = receiverId,
                SenderId = senderId
            };

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }

        public async Task<Result> FriendRequestReceived(Guid receiverId, UserViewModel senderViewModel)
        {
            var request = new FriendRequestReceivedRequest
            {
                ReceiverId = receiverId,
                SenderViewModel = senderViewModel
            };

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }
    }
}
