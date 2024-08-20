using Chats.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chats.EF.Configurations
{
    public class MessageIdValueConverter : ValueConverter<MessageId, Guid>
    {
        public MessageIdValueConverter() : base(
            messageId => messageId.Id,
            guid => MessageId.Create(guid).Value)
        { }
    }
}