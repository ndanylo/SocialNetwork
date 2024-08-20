using MediatR;
using CSharpFunctionalExtensions;
using Notifications.Domain.ValueObjects;
using Notifications.Domain.Entities;

namespace Notifications.Application.Commands.CreateNotification
{
    public class CreateNotificationCommand : IRequest<Result<Notification>>
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
    }
}
