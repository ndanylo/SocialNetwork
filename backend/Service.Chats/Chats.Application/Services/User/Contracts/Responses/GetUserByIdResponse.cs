using Chats.Application.ViewModels;

namespace MessageBus.Contracts.Responses
{
    public class GetUserByIdResponse
    {
        public UserViewModel User { get; set; } = new UserViewModel();
    }
}