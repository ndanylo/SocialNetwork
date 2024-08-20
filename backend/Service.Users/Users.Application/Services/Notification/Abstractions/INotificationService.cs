using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Posts.Application.Services.Abstractions
{
    public interface INotificationService
    {
        Task<Result> FriendRemoved(Guid UserId, Guid FriendId);
        Task<Result> FriendRequestAccepted(Guid ReceiverId, Guid SenderId);
        Task<Result> FriendRequestCancelled(Guid ReceiverId, Guid SenderId);
        Task<Result> FriendRequestDeclined(Guid ReceiverId, Guid SenderId);
        Task<Result> FriendRequestReceived(Guid ReceiverId, UserViewModel senderViewModel);
    }
}
