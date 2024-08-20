namespace MessageBus.Contracts.Requests
{
    public class IsPostLikedByUserRequest
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}