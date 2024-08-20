using Chats.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chats.EF.Configurations
{
    public class ChatUserIdValueConverter : ValueConverter<ChatUserId, string>
    {
        public ChatUserIdValueConverter() : base(
            chatUserId => $"{chatUserId.UserId}!{chatUserId.ChatId}",
            value => ConvertFromString(value))
        { }

        private static ChatUserId ConvertFromString(string value)
        {
            var parts = value.Split('!');
            if (parts.Length != 2)
                throw new InvalidOperationException($"Invalid ChatUserId format. {value}");

            var userId = UserId.Create(Guid.Parse(parts[0])).Value;
            var chatId = ChatId.Create(Guid.Parse(parts[1])).Value;

            return ChatUserId.Create(userId, chatId).Value;
        }
    }
}
