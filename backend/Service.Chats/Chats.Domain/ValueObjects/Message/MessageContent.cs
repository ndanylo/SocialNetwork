using CSharpFunctionalExtensions;

namespace Chats.Domain.ValueObjects
{
    public class MessageContent : ValueObject
    {
        public string Content { get; }

        private MessageContent(string content)
        {
            Content = content;
        }

        public static Result<MessageContent> Create(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Result.Failure<MessageContent>("Message content cannot be empty.");
            }

            return Result.Success(new MessageContent(content));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Content;
        }

        public static implicit operator string(MessageContent messageContent)
        {
            return messageContent.Content;
        }

        public override string ToString()
        {
            return Content;
        }

        public static MessageContent Empty
        {
            get
            {
                return new MessageContent(string.Empty);
            }
        }
    }
}