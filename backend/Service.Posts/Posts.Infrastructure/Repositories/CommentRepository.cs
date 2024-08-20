using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;
using Posts.Infrastructure.EF;

namespace Posts.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostDbContext _context;

        public CommentRepository(PostDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> AddCommentAsync(Comment comment)
        {
            try
            {
                await _context.Comments.AddAsync(comment);
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<List<Comment>>> GetCommentsByPostId(PostId postId)
        {
            try
            {
                var comments = await _context.Comments
                    .Where(c => c.PostId == postId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return Result.Success(comments);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<Comment>>(ex.Message);
            }
        }

        public async Task<Result<Comment>> GetCommentByIdAsync(CommentId commentId)
        {
            try
            {
                var comment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    return Result.Failure<Comment>("Comment was not found");
                }

                return Result.Success(comment);
            }
            catch(Exception ex)
            {
                return Result.Failure<Comment>(ex.Message);
            }
        }

        public Result RemoveComment(Comment comment)
        {
            try
            {
                _context.Comments.Remove(comment);
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
