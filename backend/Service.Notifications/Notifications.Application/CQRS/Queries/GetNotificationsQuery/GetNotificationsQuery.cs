using MediatR;
using Notifications.Application.ViewModels;
using CSharpFunctionalExtensions;

namespace Notifications.Application.Queries.GetNotifications
{
    public class GetNotificationsQuery : IRequest<Result<List<NotificationViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}