using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Chats.Domain.Entities
{
    public class Message : Entity<MessageId>
    {
        public ChatUserId ChatUserId { get; private set; }
        public ChatId ChatId { get; private set; }
        public virtual Chat ChatRoom { get; private set; }
        public MessageContent Content { get; private set; }
        public DateTime Timestamp { get; private set; } = DateTime.Now;
        public ICollection<ChatUser> ReadBy { get; private set; } = new List<ChatUser>();

        private Message()
        {
            ChatRoom = Chat.Default;
            ChatUserId = ChatUserId.Empty;
            ChatId = ChatId.Empty;
            Content = MessageContent.Empty;
        }

        private Message(MessageId id,
                        ChatUserId chatUserId,
                        ChatId chatId,
                        Chat chatRoom,
                        MessageContent content) : base(id)
        {
            Id = id;
            ChatUserId = chatUserId;
            ChatRoom = chatRoom;
            ChatId = chatId;
            Content = content;
            Timestamp = DateTime.UtcNow;
        }

        public static Result<Message> Create(MessageId id,
                                            ChatUserId chatUserId,
                                            ChatId chatRoomId,
                                            Chat chatRoom,
                                            MessageContent content)
        {
            if (id == null)
            {
                return Result.Failure<Message>("Message ID content cannot be null.");
            }
            if (chatRoom == null)
            {
                return Result.Failure<Message>("ChatRoom cannot be null.");
            }
            if (chatUserId == null)
            {
                return Result.Failure<Message>("User ID cannot be null.");
            }
            if (chatRoomId == null)
            {
                return Result.Failure<Message>("Chat ID cannot be null.");
            }
            if (content == null)
            {
                return Result.Failure<Message>("Message content cannot be null.");
            }

            var message = new Message(id, chatUserId, chatRoomId, chatRoom, content);

            return Result.Success(message);
        }

        public void MarkAsReadBy(ChatUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!ReadBy.Contains(user))
            {
                ReadBy.Add(user);
            }
        }

        public static Message Default
        {
            get
            {
                return new Message
                {
                    ChatUserId = ChatUserId.Empty,
                    ChatId = ChatId.Empty,
                    Content = MessageContent.Empty
                };
            }
        }
    }
}