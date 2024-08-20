using CSharpFunctionalExtensions;
using Notifications.Domain.Abstractions;

namespace Notifications.Domain.ValueObjects
{
    public class PostId : GuidIdValueObject<PostId>
    {
        private PostId(Guid id) : base(id) { }

        public static Result<PostId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<PostId>("Invalid PostId format.");
            }

            return Result.Success(new PostId(id));
        }
    }
}