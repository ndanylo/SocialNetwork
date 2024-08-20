using Authorization.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Domain.Entities
{
    public class UserCredentials : IdentityUser<UserId>
    {
        private UserCredentials() : base() { }

        private UserCredentials(string email, UserId userId)
        {
            Email = email;
            UserName = email;
            Id = userId;
        }

        public static Result<UserCredentials> Create(string email, UserId userId)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result.Failure<UserCredentials>("email can not be null or empty");
            }

            if (userId == null || userId == UserId.Empty)
            {
                return Result.Failure<UserCredentials>("id can not be null or empty");
            }
            return Result.Success(new UserCredentials(email, userId));
        }
    }
}