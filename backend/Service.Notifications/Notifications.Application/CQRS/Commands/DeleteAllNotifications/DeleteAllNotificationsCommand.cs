using MediatR;
using CSharpFunctionalExtensions;

namespace Notifications.Application.Commands.DeleteAllNotifications
{
    public class DeleteAllNotificationsCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
    }
}