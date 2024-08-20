using Notifications.Application.ViewModels;

namespace Notifications.Application.Hubs.Abstraction
{
    public interface IChatHub
    {
        Task ReceiveMessage(MessageViewModel message);
        Task ReadChat(Guid chatRoomId);
        Task FriendRemoved(Guid userId);
        Task FriendRequestReceived(UserViewModel user);
        Task FriendRequestAccepted(Guid userId);
        Task FriendRequestCancelled(Guid userId);
        Task FriendRequestDeclined(Guid userId);
        Task SetMessageRead(Guid chatRoomId, Guid messageId, UserViewModel reader);
        Task ReceiveNotification(NotificationViewModel notification);
    }
}
