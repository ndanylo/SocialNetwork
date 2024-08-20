using Notifications.Application.ViewModels;

namespace MessageBus.Contracts.Requests
{
    public class SendMessageRequest
    {
        public Guid ReceiverId { get; set; }
        public MessageViewModel Message { get; set; } = new MessageViewModel();
    }
}