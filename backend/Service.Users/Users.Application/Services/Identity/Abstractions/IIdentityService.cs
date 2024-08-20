using CSharpFunctionalExtensions;

namespace Users.Application.Services.Abstractions
{
    public interface IIdentityService
    {
        Task<Result<Guid>> RegisterUserAsync(string email, string password);
    }
}
