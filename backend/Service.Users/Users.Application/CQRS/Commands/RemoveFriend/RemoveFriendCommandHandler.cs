using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.Commands.RemoveFriend;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;

namespace OnlineChat.Application.Users.Commands.RemoveFriend
{
    public class RemoveFriendCommandHandler : IRequestHandler<RemoveFriendCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly INotificationService _notificationService;

        public RemoveFriendCommandHandler(INotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid user ID.");
            }

            var friendIdResult = UserId.Create(request.FriendId);
            if (friendIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid friend ID.");
            }

            var removeFriendResult = await _unitOfWork.Users.RemoveFriendAsync(userIdResult.Value, friendIdResult.Value);
            if(removeFriendResult.IsFailure)
            {
                return Result.Failure<Unit>(removeFriendResult.Error);
            }
            
            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<Unit>(saveChangesResult.Error);
            }

            var notificationResult = await _notificationService.FriendRemoved(userIdResult.Value, friendIdResult.Value);
            if (notificationResult.IsFailure)
            {
                return Result.Failure<Unit>($"Failed to send notification: {notificationResult.Error}");
            }
            return Result.Success(Unit.Value);
            
        }
    }
}
