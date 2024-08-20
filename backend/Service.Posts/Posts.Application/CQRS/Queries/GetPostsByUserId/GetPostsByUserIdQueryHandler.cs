using AutoMapper;
using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;

namespace Posts.Application.Queries.GetPostsByUserId
{
    public class GetPostsByUserIdQueryHandler : IRequestHandler<GetPostsByUserIdQuery, Result<List<PostViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetPostsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<Result<List<PostViewModel>>> Handle(GetPostsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<List<PostViewModel>>("Invalid user ID.");
            }

            var userResult = await _userService.GetUserByIdAsync(userIdResult.Value);
            if (userResult.IsFailure)
            {
                return Result.Failure<List<PostViewModel>>("Failed to retrieve user information.");
            }

            var userIds = new List<UserId>
            {
                userIdResult.Value
            };

            var getPostsResult = await _unitOfWork.Posts.GetPostsByUserIdsAsync(userIds);
            if(getPostsResult.IsFailure)
            {
                return Result.Failure<List<PostViewModel>>(getPostsResult.Error);
            }
            var posts = getPostsResult.Value;

            try
            {
                var postViewModels = _mapper.Map<List<PostViewModel>>(posts, opt => opt.Items["User"] = userResult.Value);
                return Result.Success(postViewModels);
            }
            catch
            {
                return Result.Failure<List<PostViewModel>>("Failed to retrieve posts.");
            }
        }
    }
}
