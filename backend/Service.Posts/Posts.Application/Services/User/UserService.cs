using CSharpFunctionalExtensions;
using MassTransit;
using MessageBus.Contracts.Requests;
using MessageBus.Contracts.Responses;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.ValueObjects;

namespace Posts.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IRequestClient<GetUserByIdRequest> _userClient;
        private readonly IRequestClient<GetUserListByIdRequest> _getUserListByIdClient;
        private readonly IRequestClient<GetUserFriendsRequest> _friendsClient;

        public UserService(IRequestClient<GetUserByIdRequest> userClient,
                        IRequestClient<GetUserListByIdRequest> getUserListByIdClient,
                        IRequestClient<GetUserFriendsRequest> friendsClient)
        {
            _userClient = userClient;
            _friendsClient = friendsClient;
            _getUserListByIdClient = getUserListByIdClient;
        }

        public async Task<Result<UserViewModel>> GetUserByIdAsync(UserId userId)
        {
            var response = await _userClient.GetResponse<GetUserByIdResponse>(new GetUserByIdRequest { UserId = userId.Id });
            return response.Message.User != null
                ? Result.Success(response.Message.User)
                : Result.Failure<UserViewModel>("User not found.");
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
                return Result.Success(users ?? new List<UserViewModel>());
            }
            catch
            {
                return Result.Failure<IEnumerable<UserViewModel>>("Error while retrieving user details.");
            }
        }

        public async Task<Result<List<UserId>>> GetFriendIdsAsync(UserId userId)
        {
            var response = await _friendsClient.GetResponse<GetUserFriendsResponse>(new GetUserFriendsRequest { UserId = userId.Id });
            if (response.Message.Friends == null)
            {
                return Result.Failure<List<UserId>>("Failed to retrieve friends.");
            }

            var userIds = response.Message.Friends.Select(friend => UserId.Create(friend.Id).Value).ToList();
            return Result.Success(userIds);
        }
    }
}
