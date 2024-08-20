namespace MessageBus.Contracts.Requests
{
    public class FriendRequestDeclinedRequest
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}
