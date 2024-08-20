namespace Chats.Application.ViewModels
{
    public class ChatViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<UserViewModel>? Users { get; set; }
        public List<MessageViewModel>? Messages { get; set; }
        public int UnreadMessagesCount { get; set; }
        public MessageViewModel? LastMessage { get; set; }
        public bool IsGroupChat { get; set; }
    }
}