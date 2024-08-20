using Notifications.Application.ViewModels;

namespace MessageBus.Contracts.Responses
{
    public class GetPostByIdResponse
    {
        public PostViewModel Post { get; set; } = new PostViewModel();
    }
}