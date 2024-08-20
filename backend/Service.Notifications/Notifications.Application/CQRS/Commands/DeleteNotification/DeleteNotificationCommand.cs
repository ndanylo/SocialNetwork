using MediatR;
using CSharpFunctionalExtensions;

namespace Notifications.Application.Commands.DeleteNotification
{
    public class DeleteNotificationCommand : IRequest<Result<bool>>
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }
}