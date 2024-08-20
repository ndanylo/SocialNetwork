using CSharpFunctionalExtensions;

namespace Posts.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IPostRepository Posts { get; }
        ICommentRepository Comments { get; }
        ILikeRepository Likes { get; }
        Task<Result> SaveChangesAsync();
    }
}
