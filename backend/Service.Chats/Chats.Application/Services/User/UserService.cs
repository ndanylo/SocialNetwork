using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Chats.Application.ViewModels;

namespace Chats.Application.Services.Abstractions
{
    public class UserService : IUserService
    {
        private readonly IRequestClient<GetUserListByIdRequest> _getUserListByIdClient;
        private readonly IRequestClient<GetUserByIdRequest> _getUserByIdClient;
        private readonly ILogger<UserService> _logger;

        public UserService(IRequestClient<GetUserListByIdRequest> getUserListByIdClient,
                           IRequestClient<GetUserByIdRequest> getUserByIdClient,
                           ILogger<UserService> logger)
        {
            _getUserListByIdClient = getUserListByIdClient;
            _getUserByIdClient = getUserByIdClient;
            _logger = logger;
        }

        public async Task<Result<UserViewModel>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var response = await _getUserByIdClient.GetResponse<GetUserByIdResponse>(new GetUserByIdRequest
                {
                    UserId = userId
                });

                var user = response.Message.User;

                if (user != null)
                {
                    _logger.LogInformation("Successfully retrieved details for user {UserId}", userId);
                    return Result.Success(user);
                }
                else
                {
                    _logger.LogWarning("No user details found for user ID {UserId}", userId);
                    return Result.Failure<UserViewModel>("No user details found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving user details for user ID {UserId}", userId);
                return Result.Failure<UserViewModel>("Error while retrieving user details.");
            }
        }

        public async Task<Result<IEnumerable<UserViewModel>>> GetUserListByIdAsync(IEnumerable<Guid> userIds)
        {
            try
            {
                var request = new GetUserListByIdRequest
                {
                    UserIds = userIds
                };

                var response = await _getUserListByIdClient.GetResponse<GetUserListByIdResponse>(request);

                var users = response.Message.Users;
                if (users != null && users.Any())
                {
                    _logger.LogInformation("Successfully retrieved details for {Count} users", users.Count());
                }
                else
                {
                    _logger.LogWarning("No user details found for the provided user IDs.");
                }
                return Result.Success(users ?? new List<UserViewModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving user details.");
                return Result.Failure<IEnumerable<UserViewModel>>("Error while retrieving user details.");
            }
        }
    }
}
