using CSharpFunctionalExtensions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Domain.Abstractions
{
    public interface IUserRepository
    {
        Task<Result<User?>> GetUserByIdAsync(UserId userId);
        Task<Result<List<User>>> GetAllUsersAsync(UserId currentUserId);
        Task<Result<User>> AddUserAsync(User user);
        Task<Result<List<User>>> GetUsersByIds(List<UserId> userIds);
        Task<Result<List<User>>> GetUserFriendsAsync(UserId userId);
        Task<Result> AddFriendAsync(UserId userId, UserId friendId);
        Task<Result> RemoveFriendAsync(UserId userId, UserId friendId);
        Task<Result<bool>> AreFriendsAsync(UserId user1, UserId user2);
    }
}