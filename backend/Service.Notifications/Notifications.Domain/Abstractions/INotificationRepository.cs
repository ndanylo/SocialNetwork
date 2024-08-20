using CSharpFunctionalExtensions;
using Notifications.Domain.Entities;
using Notifications.Domain.ValueObjects;

namespace Notifications.Domain.Abstractions
{
    public interface INotificationRepository
    {
        Task<Result> AddNotificationAsync(Notification notification);
        Task<Result> RemoveNotificationAsync(NotificationId notificationId, UserId userId);
        Task<Result> RemoveAllNotificationsAsync(UserId userId);
        Task<Result<IEnumerable<Notification>>> GetNotificationsAsync(UserId userId);
        Task<Result<Notification>> GetNotificationByIdAsync(NotificationId notificationId, UserId userId);
        Task<Result<Notification?>> GetNotificationByDetailsAsync(UserId userId, PostId postId, NotificationType type);
        Task<Result> SaveChangesAsync();
    }
}