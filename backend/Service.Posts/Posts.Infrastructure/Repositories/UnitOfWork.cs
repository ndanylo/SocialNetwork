using CSharpFunctionalExtensions;
using Posts.Domain.Abstractions;
using Posts.Infrastructure.EF;

namespace Posts.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PostDbContext _context;

        public UnitOfWork(PostDbContext context,
                        ICommentRepository commentRepository,
                        ILikeRepository likeRepository,
                        IPostRepository postRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Comments = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            Likes = likeRepository ?? throw new ArgumentNullException(nameof(likeRepository));
            Posts = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        }

        public ICommentRepository Comments { get; private set; }
        public ILikeRepository Likes { get; private set; }
        public IPostRepository Posts { get; private set; }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
