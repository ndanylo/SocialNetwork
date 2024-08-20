using CSharpFunctionalExtensions;
using Notifications.Application.ViewModels;
using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Services.Abstraction
{
    public interface IUserService
    {
        Task<Result<UserViewModel>> GetUserByIdAsync(UserId userId);
    }
}
