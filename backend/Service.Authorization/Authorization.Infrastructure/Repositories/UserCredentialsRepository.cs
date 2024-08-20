using Authorization.Domain.Abstractions;
using Authorization.Domain.Entities;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Infrastructure.Repositories
{
    public class UserCredentialsRepository : IUserCredentialsRepository
    {
        private readonly UserManager<UserCredentials> _userManager;
        private readonly SignInManager<UserCredentials> _signInManager;

        public UserCredentialsRepository(UserManager<UserCredentials> userManager, SignInManager<UserCredentials> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result<IdentityResult>> RegisterAsync(UserCredentials user, string password)
        {
            try
            {
                var userCreationResult = await _userManager.CreateAsync(user, password);
                return Result.Success(userCreationResult);
            }
            catch(Exception ex)
            {
                return Result.Failure<IdentityResult>(ex.Message);
            }
        }

        public async Task<Result<SignInResult>> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return SignInResult.Failed;
                }

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
                return Result.Success(signInResult);
            }
            catch(Exception ex)
            {
                return Result.Failure<SignInResult>(ex.Message);
            }
        }

        public async Task<Result<UserCredentials?>> FindByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return Result.Success(user);
            }
            catch(Exception ex)
            {
                return Result.Failure<UserCredentials?>(ex.Message);
            }
        }
    }
}
