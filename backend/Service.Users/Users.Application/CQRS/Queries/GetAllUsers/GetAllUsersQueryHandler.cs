using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.Queries.GetAllUsers;
using Users.Application.ViewModels;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using AutoMapper;

namespace OnlineChat.Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<UserViewModel>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<List<UserViewModel>>("Invalid user ID.");
            }

            try
            {
                var getUsersResult = await _unitOfWork.Users.GetAllUsersAsync(userIdResult.Value);
                if(getUsersResult.IsFailure)
                {
                    return Result.Failure<List<UserViewModel>>(getUsersResult.Error);
                }
                var users = getUsersResult.Value;

                var userViewModels = _mapper.Map<List<UserViewModel>>(users);
                return Result.Success(userViewModels);
            }
            catch
            {
                return Result.Failure<List<UserViewModel>>("An error occurred while mapping users.");
            }
        }
    }
}
