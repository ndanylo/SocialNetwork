using CSharpFunctionalExtensions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Domain.Abstractions
{
    public interface IPostRepository
    {
        Task<Result<Post>> GetPostByIdAsync(PostId postId);
        Task<Result> AddPostAsync(Post post);
        Task<Result<List<Post>>> GetPostsByUserIdsAsync(List<UserId> userIds);
        Task<Result<bool>> IsPostLikedByUserAsync(PostId postId, UserId userId);
    }
}