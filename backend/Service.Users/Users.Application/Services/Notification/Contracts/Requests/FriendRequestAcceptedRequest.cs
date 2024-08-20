namespace MessageBus.Contracts.Requests
{
    public class FriendRequestAcceptedRequest
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
    }
}
