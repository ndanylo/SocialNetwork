using MediatR;
using AutoMapper;
using CSharpFunctionalExtensions;
using Users.Application.Queries.GetFriends;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Users.Domain.Entities;
using Users.Application.ViewModels;

namespace OnlineChat.Application.Users.Queries.GetFriends
{
    public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, Result<List<UserViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFriendsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<UserViewModel>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>("Invalid user ID.");
            }

            var getFriendsResult = await _unitOfWork.Users.GetUserFriendsAsync(userIdResult.Value);
            if(getFriendsResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>(getFriendsResult.Error);
            }
            List<User> friends = getFriendsResult.Value;
            
            try
            {
                var friendsViewModel = _mapper.Map<List<UserViewModel>>(friends);
                return Result.Success(friendsViewModel);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<UserViewModel>>(ex.Message);
            }
        }
    }
}
