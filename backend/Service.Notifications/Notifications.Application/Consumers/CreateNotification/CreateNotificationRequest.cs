using Notifications.Domain.ValueObjects;

namespace MessageBus.Contracts.Requests
{
    public class CreateNotificationRequest
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
    }
}
