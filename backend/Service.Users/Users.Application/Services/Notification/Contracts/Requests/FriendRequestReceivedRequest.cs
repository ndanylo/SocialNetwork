using Users.Application.ViewModels;

namespace MessageBus.Contracts.Requests
{
    public class FriendRequestReceivedRequest
    {
        public Guid ReceiverId { get; set; }
        public UserViewModel SenderViewModel { get; set; } = new UserViewModel();
    }
}
