using CSharpFunctionalExtensions;
using Notifications.Domain.ValueObjects;

namespace Notifications.Domain.Entities
{
    public class Notification : Entity<NotificationId>
    {
        public UserId UserId { get; private set; }
        public PostId PostId { get; private set; }
        public NotificationContent Content { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public NotificationType Type { get; private set; }

        private Notification()
        {
            UserId = UserId.Empty;
            PostId = PostId.Empty;
            Content = NotificationContent.Empty;
        }

        private Notification(NotificationId id,
                            UserId userId,
                            PostId postId,
                            NotificationContent content,
                            NotificationType type) : base(id)
        {
            Id = id;
            UserId = userId;
            PostId = postId;
            Content = content;
            Type = type;
            IsRead = false;
            CreatedAt = DateTime.Now;
        }

        public static Result<Notification> Create(NotificationId id,
                                                UserId userId,
                                                PostId postId,
                                                NotificationContent content,
                                                NotificationType type)
        {
            if (id == null)
                return Result.Failure<Notification>("Notification ID cannot be null.");

            if (userId == null)
                return Result.Failure<Notification>("User ID cannot be null.");

            if (postId == null)
                return Result.Failure<Notification>("Post ID cannot be null.");

            if (content == null)
                return Result.Failure<Notification>("Notification content cannot be null.");

            var notification = new Notification(id, userId, postId, content, type);
            return Result.Success(notification);
        }

        public static Notification Default
        {
            get
            {
                return new Notification
                {
                    UserId = UserId.Empty,
                    PostId = PostId.Empty,
                    Content = NotificationContent.Empty,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
            }
        }
    }
}