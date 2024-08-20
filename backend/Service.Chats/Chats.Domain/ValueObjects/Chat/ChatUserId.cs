using CSharpFunctionalExtensions;

namespace Chats.Domain.ValueObjects
{
    public class ChatUserId : ValueObject
    {
        public UserId UserId { get; private set; }
        public ChatId ChatId { get; private set; }

        private ChatUserId(UserId userId, ChatId chatId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ChatId = chatId ?? throw new ArgumentNullException(nameof(chatId));
        }

        public static Result<ChatUserId> Create(UserId userId, ChatId chatId)
        {
            if (userId == null)
                return Result.Failure<ChatUserId>("UserId cannot be null.");

            if (chatId == null)
                return Result.Failure<ChatUserId>("ChatId cannot be null.");

            return Result.Success(new ChatUserId(userId, chatId));
        }

        public static ChatUserId Empty => new ChatUserId(UserId.Empty, ChatId.Empty);

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return UserId;
            yield return ChatId;
        }
    }
}
