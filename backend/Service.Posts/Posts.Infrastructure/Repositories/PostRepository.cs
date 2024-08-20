using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;
using Posts.Infrastructure.EF;

namespace Posts.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _context;

        public PostRepository(PostDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Post>> GetPostByIdAsync(PostId postId)
        {
            try
            {
                var post = await _context.Posts
                    .Include(p => p.Likes)
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return Result.Failure<Post>($"Post with ID {postId} not found.");
                }

                return Result.Success(post);
            }
            catch(Exception ex)
            {
                return Result.Failure<Post>(ex.Message);
            }
        }

        public async Task<Result<List<Post>>> GetPostsByUserIdsAsync(List<UserId> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return Result.Success(new List<Post>());
            }
            try 
            {
                var posts = await _context.Posts
                    .Include(p => p.Likes)
                    .Include(p => p.Comments)
                    .Where(p => userIds.Contains(p.UserId))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
                
                return Result.Success(posts);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<Post>>(ex.Message);
            }
        }

        public async Task<Result> AddPostAsync(Post post)
        {
            if (post == null)
            {
                return Result.Failure(nameof(post));
            }
            
            try
            {
                await _context.Posts.AddAsync(post);
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
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
