using MediatR;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;

namespace OnlineChat.Application.FriendRequests.Commands.CancelFriendRequest
{
    public class CancelFriendRequestCommandHandler : IRequestHandler<CancelFriendRequestCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CancelFriendRequestCommandHandler(INotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(CancelFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var senderIdResult = UserId.Create(request.SenderId);
            if (senderIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid sender ID.");
            }

            var receiverIdResult = UserId.Create(request.ReceiverId);
            if (receiverIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid receiver ID.");
            }

            var getFriendRequestResult = await _unitOfWork.FriendRequests
                .GetFriendRequestBySenderAndReceiverIdsAsync(senderIdResult.Value, receiverIdResult.Value);
            
            if (getFriendRequestResult.IsFailure)
            {
                return Result.Failure<Unit>($"Friend request from sender ID {request.SenderId} to receiver ID {request.ReceiverId} not found.");
            }
            if (getFriendRequestResult.Value == null)
            {
                return Result.Failure<Unit>("Friend request not found");
            }

            var friendRequest = getFriendRequestResult.Value;

            if (friendRequest.SenderId != request.SenderId)
            {
                return Result.Failure<Unit>("You are not authorized to cancel this friend request.");
            }

            var removeFriendResult = _unitOfWork.FriendRequests.RemoveFriendRequest(friendRequest);
            if(removeFriendResult.IsFailure)
            {
                return Result.Failure<Unit>(removeFriendResult.Error);
            }

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<Unit>(saveChangesResult.Error);
            }

            var notificationResult = await _notificationService
                .FriendRequestCancelled(receiverIdResult.Value,senderIdResult.Value);
            if (notificationResult.IsFailure)
            {
                return Result.Failure<Unit>($"Failed to send notification: {notificationResult.Error}");
            }

            return Result.Success(Unit.Value);
        }
    }
}