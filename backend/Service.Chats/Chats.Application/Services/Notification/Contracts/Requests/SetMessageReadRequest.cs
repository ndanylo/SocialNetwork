using Chats.Application.ViewModels;

namespace MessageBus.Contracts.Requests
{
    public class SetMessageReadRequest
    {
        public Guid ChatRoomId { get; set; }
        public Guid MessageId { get; set; }
        public UserViewModel Reader { get; set; } = new UserViewModel();
    }
}
