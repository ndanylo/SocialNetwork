using CSharpFunctionalExtensions;
using Notifications.Domain.Abstractions;

namespace Posts.Domain.ValueObjects
{
    public class LikeId : GuidIdValueObject<LikeId>
    {
        private LikeId(Guid id) : base(id) { }

        public static Result<LikeId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<LikeId>("Invalid NotificationId format.");
            }

            return Result.Success(new LikeId(id));
        }
    }
}