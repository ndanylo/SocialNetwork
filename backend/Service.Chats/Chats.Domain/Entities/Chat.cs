using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Chats.Domain.Entities
{
    public class Chat : Entity<ChatId>
    {
        public ChatRoomName Name { get; private set; }
        public bool IsGroupChat { get; private set; } = false;
        public List<ChatUser> Users { get; private set; } = new List<ChatUser>();
        public List<Message> Messages { get; private set; } = new List<Message>();
        private Chat()
        {
            Name = ChatRoomName.Empty;
        }

        private Chat(ChatId id,
                        ChatRoomName name,
                        bool isGroupChat,
                        IEnumerable<ChatUser> users) : base(id)
        {
            Name = name;
            IsGroupChat = isGroupChat;
            Users.AddRange(users);
        }

        public static Result<Chat> Create(ChatId id,
                                        ChatRoomName name,
                                        bool isGroupChat,
                                        IEnumerable<ChatUser> users)
        {
            if (id == null)
                return Result.Failure<Chat>("Id cannot be empty");

            if (name == null)
                return Result.Failure<Chat>("Chat name cannot be null");

            if (users == null || !users.Any())
                return Result.Failure<Chat>("Chat must have users");

            if (users.Count() < 3 && isGroupChat)
                return Result.Failure<Chat>("Group must have at least 3 users");

            var chatRoom = new Chat(id, name, isGroupChat, users);

            return Result.Success(chatRoom);
        }

        public void AddMessage(Message message)
        {
            Messages.Add(message ?? throw new ArgumentNullException(nameof(message)));
        }

        public void RemoveUser(ChatUser user)
        {
            Users.Remove(user);
        }

        public void AddUser(ChatUser user)
        {
            Users.Add(user);
        }

        public static Chat Default
        {
            get
            {
                return new Chat
                {
                    Name = ChatRoomName.Empty,
                    IsGroupChat = false
                };
            }
        }
    }
}