using Chats.Application.ViewModels;
using CSharpFunctionalExtensions;

namespace Chats.Application.Services.Abstractions
{
    public interface IUserService
    {
        Task<Result<UserViewModel>> GetUserByIdAsync(Guid userId);
        Task<Result<IEnumerable<UserViewModel>>> GetUserListByIdAsync(IEnumerable<Guid> userIds);
    }
}