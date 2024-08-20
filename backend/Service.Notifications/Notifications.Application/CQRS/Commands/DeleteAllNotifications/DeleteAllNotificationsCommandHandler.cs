using MediatR;
using CSharpFunctionalExtensions;
using Notifications.Domain.ValueObjects;
using Notifications.Domain.Abstractions;

namespace Notifications.Application.Commands.DeleteAllNotifications
{
    public class DeleteAllNotificationsCommandHandler : IRequestHandler<DeleteAllNotificationsCommand, Result<Unit>>
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteAllNotificationsCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Result<Unit>> Handle(DeleteAllNotificationsCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid user ID.");
            }

            var removeNotificationsResult = await _notificationRepository.RemoveAllNotificationsAsync(userIdResult.Value);
            if(removeNotificationsResult.IsFailure)
            {
                return Result.Failure<Unit>(removeNotificationsResult.Error);
            }

            var saveChangesResult = await _notificationRepository.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<Unit>("Error while deleting notification");
            }

            return Result.Success(Unit.Value);
        }
    }
}
