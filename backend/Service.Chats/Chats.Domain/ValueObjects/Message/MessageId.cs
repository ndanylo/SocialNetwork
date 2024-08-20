using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;

namespace Chats.Domain.ValueObjects
{
    public class MessageId : GuidIdValueObject<MessageId>
    {
        private MessageId(Guid id) : base(id) { }

        public static Result<MessageId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<MessageId>("Invalid NotificationId format.");
            }

            return Result.Success(new MessageId(id));
        }
    }
}