using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Services.Abstractions
{
    public interface IUserService
    {
        Task<Result<UserViewModel>> GetUserByIdAsync(UserId userId);
        Task<Result<List<UserId>>> GetFriendIdsAsync(UserId userId);
        Task<Result<IEnumerable<UserViewModel>>> GetUserListByIdAsync(IEnumerable<Guid> userIds);
    }
}
