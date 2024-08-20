namespace MessageBus.Contracts.Requests
{
    public class GetPostByIdRequest
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}