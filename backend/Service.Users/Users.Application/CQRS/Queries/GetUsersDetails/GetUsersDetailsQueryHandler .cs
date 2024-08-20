using MediatR;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using AutoMapper;
using Users.Application.ViewModels;

namespace Users.Application.Queries.GetUsersDetails
{
    public class GetUsersDetailsQueryHandler : IRequestHandler<GetUsersDetailsQuery, Result<List<UserViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<UserViewModel>>> Handle(GetUsersDetailsQuery request, CancellationToken cancellationToken)
        {
            var userIds = new List<UserId>();
            foreach (var id in request.UserIds)
            {
                var userIdResult = UserId.Create(id);
                if (userIdResult.IsFailure)
                {
                    return Result.Failure<List<UserViewModel>>($"Invalid user ID: {id}");
                }
                userIds.Add(userIdResult.Value);
            }

            var getUserResult = await _unitOfWork.Users.GetUsersByIds(userIds);
            if(getUserResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>(getUserResult.Error);
            }
            var users = getUserResult.Value;

            if (!users.Any())
            {
                return Result.Failure<List<UserViewModel>>("No users found.");
            }
            try
            {
                var userDetails = _mapper.Map<List<UserViewModel>>(users);
                
                return Result.Success(userDetails);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<UserViewModel>>(ex.Message);
            }
        }
    }
}