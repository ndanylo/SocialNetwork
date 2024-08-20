namespace MessageBus.Contracts.Responses
{
    public class RegisterUserResponse
    {
        public Guid UserId { get; set; }
        public bool Success { get; set; }
    }
}
