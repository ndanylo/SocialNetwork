using CSharpFunctionalExtensions;
using Notifications.Domain.Abstractions;

namespace Posts.Domain.ValueObjects
{
    public class CommentId : GuidIdValueObject<CommentId>
    {
        private CommentId(Guid id) : base(id) { }

        public static Result<CommentId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<CommentId>("Invalid NotificationId format.");
            }

            return Result.Success(new CommentId(id));
        }
    }
}