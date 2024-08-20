namespace Chats.WebApi.Requests
{
    public class CreateGroupChatRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<Guid>? UserIds { get; set; }
    }
}
