using CSharpFunctionalExtensions;
using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Users.Application.Services.Abstractions;

namespace Users.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IRequestClient<RegisterUserRequest> _registerUserRequestClient;

        public IdentityService(IRequestClient<RegisterUserRequest> registerUserRequestClient)
        {
            _registerUserRequestClient = registerUserRequestClient;
        }

        public async Task<Result<Guid>> RegisterUserAsync(string email, string password)
        {
            try
            {
                var request = new RegisterUserRequest
                {
                    Email = email,
                    Password = password
                };

                var response = await _registerUserRequestClient.GetResponse<RegisterUserResponse>(request);

                if (response.Message.Success)
                {
                    return Result.Success(response.Message.UserId);
                }
                else
                {
                    return Result.Failure<Guid>("Failed to register user.");
                }
            }
            catch
            {
                return Result.Failure<Guid>("Error while registering user.");
            }
        }
    }
}