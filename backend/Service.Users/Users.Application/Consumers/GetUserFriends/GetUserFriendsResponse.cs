using Users.Application.ViewModels;

namespace MessageBus.Contracts.Responses
{
    public class GetUserFriendsResponse
    {
        public List<UserViewModel> Friends { get; set; } = new List<UserViewModel>();
    }
}