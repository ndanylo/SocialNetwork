using Users.Application.ViewModels;

namespace MessageBus.Contracts.Responses
{
    public class UserPostsResponse
    {
        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }
}