using MediatR;
using CSharpFunctionalExtensions;
using Notifications.Domain.Entities;
using Notifications.Domain.ValueObjects;
using Notifications.Domain.Abstractions;

namespace Notifications.Application.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<Notification>>
    {
        private readonly INotificationRepository _notificationRepository;

        public CreateNotificationCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Result<Notification>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<Notification>(userIdResult.Error);
            }

            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                return Result.Failure<Notification>(postIdResult.Error);
            }

            var contentResult = NotificationContent.Create(request.Content);
            if (contentResult.IsFailure)
            {
                return Result.Failure<Notification>(contentResult.Error);
            }

            if (!Enum.IsDefined(typeof(NotificationType), request.Type))
            {
                return Result.Failure<Notification>("Invalid notification type.");
            }

            var notificationType = (NotificationType)request.Type;

            var notificationResult = Notification.Create(
                NotificationId.Create(Guid.NewGuid()).Value,
                userIdResult.Value,
                postIdResult.Value,
                contentResult.Value,
                notificationType
            );

            if (notificationResult.IsFailure)
            {
                return Result.Failure<Notification>(notificationResult.Error);
            }

            var notification = notificationResult.Value;

            var addNotificationResult = await _notificationRepository.AddNotificationAsync(notification);
            if(addNotificationResult.IsFailure)
            {
                return Result.Failure<Notification>(addNotificationResult.Error);
            }

            var saveChangesResult = await _notificationRepository.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<Notification>("Error while add the notificaiton");
            }

            return Result.Success(notification);
        }
    }
}
