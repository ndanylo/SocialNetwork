using Chats.Application.ViewModels;
using CSharpFunctionalExtensions;

namespace Chats.Application.Services.Abstractions
{
    public interface INotificationService
    {
        Task<Result> ReadChatAsync(Guid userId, Guid chatRoomId);
        Task<Result> ReceiveMessageAsync(Guid receiverId, MessageViewModel message);
        Task<Result> SetMessageReadAsync(Guid ChatRoomId, Guid MessageId, UserViewModel Reader);
    }
}
