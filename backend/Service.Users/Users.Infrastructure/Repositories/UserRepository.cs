using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using Users.Infrastructure.EF;

namespace OnlineChat.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<User?>> GetUserByIdAsync(UserId userId)
        {
            try
            {
                var users = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);
                
                return Result.Success(users);
            }
            catch(Exception ex)
            {
                return Result.Failure<User?>(ex.Message);
            }
        }

        public async Task<Result<List<User>>> GetAllUsersAsync(UserId currentUserId)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.Id != currentUserId)
                    .ToListAsync();

                return Result.Success(users); 
            }
            catch(Exception ex)
            {
                return Result.Failure<List<User>>(ex.Message);
            }
        }

        public async Task<Result<List<User>>> GetUserFriendsAsync(UserId userId)
        {
            try
            {
                var userWithFriends = await _context.Users
                    .Include(u => u.Friends)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return Result.Success(userWithFriends?.Friends.ToList() ?? new List<User>());
            }
            catch(Exception ex)
            {
                return Result.Failure<List<User>>(ex.Message);
            }
        }

        public async Task<Result> AddFriendAsync(UserId userId, UserId friendId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                var friend = await _context.Users.FindAsync(friendId);

                if (user == null || friend == null)
                {
                    return Result.Failure("User or friend not found.");
                }

                user.AddFriend(friend);
                friend.AddFriend(user);
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> RemoveFriendAsync(UserId userId, UserId friendId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                var friend = await _context.Users.FindAsync(friendId);

                if (user == null || friend == null)
                {
                    throw new KeyNotFoundException("User or friend not found.");
                }

                user.RemoveFriend(friend);
                friend.RemoveFriend(user);

                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<User>> AddUserAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                return Result.Success(user);
            }
            catch(Exception ex)
            {
                return Result.Failure<User>(ex.Message);
            }
        }

        public async Task<Result<bool>> AreFriendsAsync(UserId firstUserId, UserId secondUserId)
        {
            try
            {
                var userWithFriends = await _context.Users
                    .Include(u => u.Friends)
                    .FirstOrDefaultAsync(u => u.Id == firstUserId);

                return Result.Success(userWithFriends?.Friends.Any(f => f.Id == secondUserId) ?? false);
            }
            catch(Exception ex)
            {
                return Result.Failure<bool>(ex.Message);
            }
        }

        public async Task<Result<List<User>>> GetUsersByIds(List<UserId> userIds)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                return Result.Success(users);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<User>>(ex.Message);
            }
        }

        public async Task<Result<int>> GetFriendsCountAsync(UserId userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Friends)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return Result.Failure<int>("User not found");
                }

                return Result.Success(user.Friends.Count);
            }
            catch(Exception ex)
            {
                return Result.Failure<int>(ex.Message);
            }
        }
    }
}