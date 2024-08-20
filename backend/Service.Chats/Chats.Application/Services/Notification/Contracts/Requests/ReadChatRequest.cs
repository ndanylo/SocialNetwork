namespace MessageBus.Contracts.Requests
{
    public class ReadChatRequest
    {
        public Guid UserId { get; set; }
        public Guid ChatRoomId { get; set; }
    }
}
