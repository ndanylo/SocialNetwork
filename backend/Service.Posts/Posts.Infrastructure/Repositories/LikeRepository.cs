using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;
using Posts.Infrastructure.EF;

namespace Posts.Infrastructure.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly PostDbContext _context;

        public LikeRepository(PostDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> AddLikeAsync(Like like)
        {
            if (like == null)
            {
                return Result.Failure(nameof(like));
            }

            try
            {
                await _context.Likes.AddAsync(like);
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<Like>> GetLikeAsync(PostId postId, UserId userId)
        {
            try
            {
                var like = await _context.Likes
                    .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

                if (like == null)
                {
                    return Result.Failure<Like>($"Like with postId {postId} and userId {userId} not found.");
                }

                return Result.Success(like);
            }
            catch(Exception ex)
            {
                return Result.Failure<Like>(ex.Message);
            }
        }

        public Result RemoveLike(Like like)
        {
            if (like == null)
            {
                return Result.Failure(nameof(like));
            }

            try
            {    
                _context.Likes.Remove(like);
                return Result.Success();
            }   
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<List<Like>>> GetLikesByPostIdAsync(PostId postId)
        {
            try
            {
                 var likes = await _context.Likes
                    .Where(l => l.PostId == postId)
                    .ToListAsync();
                
                return Result.Success(likes);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<Like>>(ex.Message);
            }
        }

        public async Task<Result<bool>> IsPostLikedByUserAsync(PostId postId, UserId userId)
        {
            try
            {
                var isPostLiked = await _context.Likes
                    .AnyAsync(l => l.PostId == postId && l.UserId == userId);
                
                return Result.Success(isPostLiked);
            }
            catch(Exception ex)
            {
                return Result.Failure<bool>(ex.Message);
            }
        }
    }
}
