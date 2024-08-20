using CSharpFunctionalExtensions;

namespace Notifications.Domain.ValueObjects
{
    public class NotificationContent : ValueObject
    {
        public string Content { get; }

        private NotificationContent(string content)
        {
            Content = content;
        }

        public static Result<NotificationContent> Create(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return Result.Failure<NotificationContent>("Content cannot be null.");
            }

            return Result.Success(new NotificationContent(content));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Content;
        }

        public static implicit operator string(NotificationContent notificationContent)
        {
            return notificationContent.Content;
        }

        public override string ToString()
        {
            return Content;
        }

        public static NotificationContent Empty
        {
            get
            {
                return new NotificationContent(string.Empty);
            }
        }
    }
}