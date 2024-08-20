using MediatR;
using CSharpFunctionalExtensions;
using Notifications.Domain.Abstractions;
using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Commands.DeleteNotification
{
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, Result<bool>>
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteNotificationCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Result<bool>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<bool>("Invalid user ID.");
            }

            var notificationIdResult = NotificationId.Create(request.NotificationId);
            if (notificationIdResult.IsFailure)
            {
                return Result.Failure<bool>("Invalid notification ID.");
            }

            var removeNotificationResult = await _notificationRepository
                .RemoveNotificationAsync(notificationIdResult.Value, userIdResult.Value);
            if(removeNotificationResult.IsFailure)
            {
                return Result.Failure<bool>(removeNotificationResult.Error);
            }

            var saveChangesResult = await _notificationRepository.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<bool>("Error while deleting notifications");
            }

            return Result.Success(true);
        }
    }
}
