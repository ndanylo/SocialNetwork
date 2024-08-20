using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.Commands.AcceptFriendRequest;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;

namespace Users.Application.FriendRequests.Commands.AcceptFriendRequest
{
    public class AcceptFriendRequestCommandHandler : IRequestHandler<AcceptFriendRequestCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public AcceptFriendRequestCommandHandler(INotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
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

            try
            {
                var getFriendRequestResult = await _unitOfWork.FriendRequests
                    .GetFriendRequestBySenderAndReceiverIdsAsync(senderIdResult.Value, receiverIdResult.Value);
                if(getFriendRequestResult.IsFailure)
                {
                    return Result.Failure<Unit>($"Friend request from sender ID {request.SenderId} to receiver ID {request.ReceiverId} not found.");
                }
                if(getFriendRequestResult.Value == null)
                {
                    return Result.Failure<Unit>("Friend request not found.");
                }
                var friendRequest = getFriendRequestResult.Value;

                var addFriendResult = await _unitOfWork.Users.AddFriendAsync(receiverIdResult.Value, senderIdResult.Value);
                if(addFriendResult.IsFailure)
                {
                    return Result.Failure<Unit>(addFriendResult.Error);
                }

                var removeFriendRequest = _unitOfWork.FriendRequests.RemoveFriendRequest(friendRequest);
                if(removeFriendRequest.IsFailure)
                {
                    return Result.Failure<Unit>(removeFriendRequest.Error);
                }

                var reverseFriendRequestResult = await _unitOfWork.FriendRequests
                    .GetFriendRequestBySenderAndReceiverIdsAsync(receiverIdResult.Value, senderIdResult.Value);
                if(reverseFriendRequestResult.IsFailure)
                {
                    return Result.Failure<Unit>(reverseFriendRequestResult.Error);
                }
                var reverseFriendRequest = reverseFriendRequestResult.Value;

                if (reverseFriendRequest != null)
                {
                    var removeFriendRequestResult = _unitOfWork.FriendRequests.RemoveFriendRequest(reverseFriendRequest);
                    if(removeFriendRequestResult.IsFailure)
                    {
                        return Result.Failure<Unit>(removeFriendRequestResult.Error);
                    }
                }

                var saveChangesAsync = await _unitOfWork.SaveChangesAsync();
                if(saveChangesAsync.IsFailure)
                {
                    return Result.Failure<Unit>(saveChangesAsync.Error);
                }

                var notificationResult = await _notificationService
                    .FriendRequestAccepted(receiverIdResult.Value, senderIdResult.Value);
                if (notificationResult.IsFailure)
                {
                    return Result.Failure<Unit>($"Failed to send notification: {notificationResult.Error}");
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<Unit>($"Failed to accept friend request: {ex.Message}");
            }

            return Result.Success(Unit.Value);
        }
    }
}