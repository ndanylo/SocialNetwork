namespace MessageBus.Contracts.Requests
{
    public class FriendRequestCancelledRequest
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
    }
}
