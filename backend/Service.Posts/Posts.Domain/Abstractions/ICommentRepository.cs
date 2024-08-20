using CSharpFunctionalExtensions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Domain.Abstractions
{
    public interface ICommentRepository
    {
        Task<Result> AddCommentAsync(Comment comment);
        Task<Result<List<Comment>>> GetCommentsByPostId(PostId postId);
        Task<Result<Comment>> GetCommentByIdAsync(CommentId commentId);
        Result RemoveComment(Comment comment);
    }
}