using Authorization.Domain.Entities;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Domain.Abstractions
{
    public interface IUserCredentialsRepository
    {
        Task<Result<IdentityResult>> RegisterAsync(UserCredentials user, string password);
        Task<Result<SignInResult>> LoginAsync(string username, string password);
        Task<Result<UserCredentials?>> FindByEmailAsync(string email);
    }
}
