using CSharpFunctionalExtensions;
using Notifications.Domain.Abstractions;

namespace Notifications.Domain.ValueObjects
{
    public class NotificationId : GuidIdValueObject<NotificationId>
    {
        private NotificationId(Guid id) : base(id) { }

        public static Result<NotificationId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<NotificationId>("Invalid NotificationId format.");
            }

            return Result.Success(new NotificationId(id));
        }
    }
}