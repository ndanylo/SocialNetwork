using CSharpFunctionalExtensions;
using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Microsoft.Extensions.Logging;
using Notifications.Application.Services.Abstraction;
using Notifications.Application.ViewModels;
using Notifications.Domain.ValueObjects;

namespace Notifications.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRequestClient<GetUserByIdRequest> _getUserByIdClient;
        private readonly ILogger<UserService> _logger;

        public UserService(IRequestClient<GetUserByIdRequest> getUserByIdClient,
                           ILogger<UserService> logger)
        {
            _getUserByIdClient = getUserByIdClient;
            _logger = logger;
        }

        public async Task<Result<UserViewModel>> GetUserByIdAsync(UserId userId)
        {
            try
            {
                var response = await _getUserByIdClient.GetResponse<GetUserByIdResponse>(new GetUserByIdRequest
                {
                    UserId = userId.Id
                });

                var user = response.Message.User;

                if (user != null)
                {
                    _logger.LogInformation("User with ID {UserId} retrieved successfully.", userId.Id);
                    return Result.Success(user);
                }
                else
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId.Id);
                    return Result.Failure<UserViewModel>("User not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving user with ID {UserId}.", userId.Id);
                return Result.Failure<UserViewModel>("Error while retrieving user.");
            }
        }
    }
}