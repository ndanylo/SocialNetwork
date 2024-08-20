using MediatR;
using AutoMapper;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Users.Domain.Entities;
using Users.Application.ViewModels;

namespace OnlineChat.Application.FriendRequests.Queries.GetSentFriendRequests
{
    public class GetSentFriendRequestsQueryHandler : IRequestHandler<GetSentFriendRequestsQuery, Result<List<UserViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSentFriendRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<UserViewModel>>> Handle(GetSentFriendRequestsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>("Invalid user ID.");
            }

            var getFriendRequestResult = await _unitOfWork.FriendRequests.GetSentFriendRequestsAsync(userIdResult.Value);
            if(getFriendRequestResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>($"Error while receiving sent friend requests: {getFriendRequestResult.Error}");
            }
            List<FriendRequest> sentRequests = getFriendRequestResult.Value;

            try
            {
                var userViewModels = _mapper.Map<List<UserViewModel>>(sentRequests.Select(fr => fr.Receiver));
                return Result.Success(userViewModels);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<UserViewModel>>(ex.Message);
            }
        }
    }
}
