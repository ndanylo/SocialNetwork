using Notifications.Domain.ValueObjects;

namespace Notifications.Application.ViewModels
{
    public class NotificationViewModel
    {
        public Guid Id { get; set; }
        public UserViewModel? UserSender { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public NotificationType Type { get; set; }
        public PostViewModel? Post { get; set; }
    }
}