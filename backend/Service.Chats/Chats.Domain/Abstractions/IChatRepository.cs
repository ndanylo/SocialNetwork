using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Chats.Domain.Abstractions
{
    public interface IChatRepository
    {
        Result<Chat> CreateGroupChat(ChatRoomName chatName, List<UserId> userIds);
        Task<Result<List<Chat>>> GetUserChatsAsync(UserId userId);
        Result<Chat> CreatePrivateChat(UserId senderId, UserId receiverId, ChatRoomName chatRoomName);
        Task<Result<Chat>> GetChatRoomAsync(ChatId сhatId, UserId userId);
        Task<Result<Chat>> GetPrivateChatByUsersAsync(UserId userSenderId, UserId userReceiverId);
        Task<Result> LeaveChatAsync(ChatId сhatId, UserId userId);
    }
}