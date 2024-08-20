using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Users.Application.Services.Abstractions
{
    public interface IPostService
    {
        Task<Result<List<PostViewModel>>> GetPostsByUserIdAsync(Guid userId);
        Task<Result<bool>> IsPostLikedByUserAsync(Guid postId, Guid userId);
    }
}