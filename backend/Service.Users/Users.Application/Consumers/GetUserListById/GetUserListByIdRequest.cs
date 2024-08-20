namespace MessageBus.Contracts.Requests
{
    public class GetUserListByIdRequest
    {
        public IEnumerable<Guid> UserIds { get; set; } = new List<Guid>();
    }
}