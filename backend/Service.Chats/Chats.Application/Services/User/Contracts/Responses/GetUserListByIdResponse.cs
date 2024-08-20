using Chats.Application.ViewModels;

namespace MessageBus.Contracts.Responses
{
    public class GetUserListByIdResponse
    {
        public IEnumerable<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}