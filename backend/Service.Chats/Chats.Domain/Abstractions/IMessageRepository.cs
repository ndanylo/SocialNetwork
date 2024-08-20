using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Chats.Domain.Abstractions
{
    public interface IMessageRepository
    {
        Task<Result<List<Message>>> ReadChatMessagesAsync(ChatId сhatId, UserId userId);
        Task<Result<Message>> SendMessageAsync(Message message);
        Task<Result<int>> GetUnreadMessageCountForUserAsync(UserId userId, ChatId сhatId);
        Task<Result<List<Message>>> GetMessagesAsync(ChatId сhatId, int amountOfMessage, int count, UserId userId);
        Task<Result<Message>> GetLastMessageAsync(ChatId сhatId);
    }
}