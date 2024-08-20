using Chats.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chats.EF.Configurations
{
    public class ChatIdValueConverter : ValueConverter<ChatId, Guid>
    {
        public ChatIdValueConverter() : base(
            chatId => chatId.Id,
            guid => ChatId.Create(guid).Value)
        { }
    }
}