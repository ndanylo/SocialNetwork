using CSharpFunctionalExtensions;
using Notifications.Domain.Abstractions;

namespace Notifications.Domain.ValueObjects
{
    public class UserId : GuidIdValueObject<UserId>, IEquatable<UserId>
    {
        private UserId(Guid id) : base(id) { }

        public static Result<UserId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<UserId>("Invalid UserId format.");
            }

            return Result.Success(new UserId(id));
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UserId);
        }

        public bool Equals(UserId? other)
        {
            return other != null && Id.Equals(other.Id);
        }
    }
}