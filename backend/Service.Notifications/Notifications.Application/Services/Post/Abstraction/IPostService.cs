using CSharpFunctionalExtensions;
using Notifications.Application.ViewModels;
using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Services.Abstraction
{
    public interface IPostService
    {
        Task<Result<PostViewModel>> GetPostByIdAsync(PostId postId, UserId userId);
    }
}
