namespace Notifications.Application.ViewModels
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid ChatId { get; set; }
        public DateTime Timestamp { get; set; }
        public UserViewModel? User { get; set; }
        public List<UserViewModel>? ReadBy { get; set; }
    }
}