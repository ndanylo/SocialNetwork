using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Chats.Domain.Entities
{
    public class ChatUser : Entity<ChatUserId>
    {
        private ChatId _chatId;
        private UserId _userId;
        public UserId UserId => _userId;
        public ChatId ChatId => _chatId;

        public ICollection<Message> ReadMessages { get; private set; } = new List<Message>();

        private ChatUser() : base(ChatUserId.Empty)
        {

            _chatId = ChatId.Empty;
            _userId = UserId.Empty;
        }

        private ChatUser(ChatUserId id)
            : base(id)
        {
            _chatId = id.ChatId;
            _userId = id.UserId;
        }

        public static Result<ChatUser> Create(UserId userId, ChatId chatId)
        {
            var chatUserIdResult = ChatUserId.Create(userId, chatId);

            if (chatUserIdResult.IsFailure)
                return Result.Failure<ChatUser>(chatUserIdResult.Error);

            var chatUser = new ChatUser(chatUserIdResult.Value);

            return Result.Success(chatUser);
        }

        public static ChatUser Default
        {
            get
            {
                return new ChatUser(ChatUserId.Create(UserId.Empty, ChatId.Empty).Value);
            }
        }
    }
}
