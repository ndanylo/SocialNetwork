namespace MessageBus.Contracts.Requests
{
    public class FriendRemovedRequest
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
    }
}
