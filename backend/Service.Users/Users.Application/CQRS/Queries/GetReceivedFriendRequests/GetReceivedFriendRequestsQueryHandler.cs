using AutoMapper;
using MediatR;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Users.Application.ViewModels;

namespace Users.Application.Queries.GetReceivedFriendRequests
{
    public class GetReceivedFriendRequestsQueryHandler : IRequestHandler<GetReceivedFriendRequestsQuery, Result<List<UserViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReceivedFriendRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<UserViewModel>>> Handle(GetReceivedFriendRequestsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>("Invalid user ID.");
            }

            var getUserResult = await _unitOfWork.Users.GetUserByIdAsync(userIdResult.Value);
            if (getUserResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>($"User with ID {request.UserId} not found.");
            }
            var currentUser = getUserResult.Value;
            try
            {
                var getFriendRequestsResult = await _unitOfWork.FriendRequests.GetReceivedFriendRequestsAsync(userIdResult.Value);
                if(getFriendRequestsResult.IsFailure)
                {
                    return Result.Failure<List<UserViewModel>>(getFriendRequestsResult.Error);
                }
                var receivedRequests = getFriendRequestsResult.Value;
                
                var userIds = receivedRequests.Select(fr => fr.SenderId).ToList();
                
                var getUsersResult = await _unitOfWork.Users.GetUsersByIds(userIds);
                if(getUsersResult.IsFailure)
                {
                    return Result.Failure<List<UserViewModel>>(getUsersResult.Error);
                }
                var users = getUsersResult.Value;

                var userViewModels = _mapper.Map<List<UserViewModel>>(users);
                return Result.Success(userViewModels);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<UserViewModel>>($"Error while receiving friend requests: {ex.Message}");
            }
        }
    }
}
