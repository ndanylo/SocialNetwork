using CSharpFunctionalExtensions;

namespace Chats.Domain.ValueObjects
{
    public class ChatRoomName : ValueObject
    {
        public string Name { get; }

        private ChatRoomName(string name)
        {
            Name = name;
        }

        public static Result<ChatRoomName> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure<ChatRoomName>("Chat room name should not be empty.");
            }

            if (name.Length < 1)
            {
                return Result.Failure<ChatRoomName>("Chat room name is too short. Chat room name must > 1");
            }

            return Result.Success(new ChatRoomName(name));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Name;
        }

        public static implicit operator string(ChatRoomName сhatRoomName)
        {
            return сhatRoomName.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static ChatRoomName Empty
        {
            get
            {
                return new ChatRoomName(string.Empty);
            }
        }
    }
}