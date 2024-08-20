using CSharpFunctionalExtensions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Domain.Abstractions
{
    public interface ILikeRepository
    {
        Task<Result> AddLikeAsync(Like like);
        Task<Result<Like>> GetLikeAsync(PostId postId, UserId userId);
        Result RemoveLike(Like like);
        Task<Result<List<Like>>> GetLikesByPostIdAsync(PostId postId);
        Task<Result<bool>> IsPostLikedByUserAsync(PostId postId, UserId userId);
    }
}