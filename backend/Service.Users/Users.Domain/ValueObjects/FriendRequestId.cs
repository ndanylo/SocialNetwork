using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;

namespace Users.Domain.ValueObjects
{
    public class FriendRequestId : GuidIdValueObject<FriendRequestId>
    {
        private FriendRequestId(Guid id) : base(id) { }

        public static Result<FriendRequestId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<FriendRequestId>("Invalid NotificationId format.");
            }

            return Result.Success(new FriendRequestId(id));
        }
    }
}