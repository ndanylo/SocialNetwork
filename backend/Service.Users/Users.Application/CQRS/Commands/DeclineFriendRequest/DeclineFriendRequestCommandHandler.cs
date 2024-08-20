using MediatR;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;

namespace Users.Application.Commands.DeclineFriendRequest
{
    public class DeclineFriendRequestCommandHandler : IRequestHandler<DeclineFriendRequestCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public DeclineFriendRequestCommandHandler(INotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeclineFriendRequestCommand request, CancellationToken cancellationToken)
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

            var friendRequest = getFriendRequestResult.Value;
            if(friendRequest == null)
            {
                return Result.Failure<Unit>("Friend request was not found");
            }

            if (friendRequest.ReceiverId != request.ReceiverId)
            {
                return Result.Failure<Unit>("You are not authorized to decline this friend request.");
            }

            var removeFriendRequestResult = _unitOfWork.FriendRequests.RemoveFriendRequest(friendRequest);
            if(removeFriendRequestResult.IsFailure)
            {
                return Result.Failure<Unit>(removeFriendRequestResult.Error);
            }
            
            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<Unit>(saveChangesResult.Error);
            }

            var notificationResult = await _notificationService
                .FriendRequestAccepted(receiverIdResult.Value, senderIdResult.Value);
            if (notificationResult.IsFailure)
            {
                return Result.Failure<Unit>($"Failed to send notification: {notificationResult.Error}");
            }

            return Result.Success(Unit.Value);
        }
    }
}