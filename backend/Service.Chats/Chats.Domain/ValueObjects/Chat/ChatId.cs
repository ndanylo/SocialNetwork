using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;

namespace Chats.Domain.ValueObjects
{
    public class ChatId : GuidIdValueObject<ChatId>
    {
        private ChatId(Guid id) : base(id) { }

        public static Result<ChatId> Create(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure<ChatId>("Invalid ChatId format.");
            }

            return Result.Success(new ChatId(id));
        }
    }
}