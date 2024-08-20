using AutoMapper;
using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;

namespace Posts.Application.Queries.GetPostById
{
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result<PostViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetPostByIdQueryHandler(IUnitOfWork unitOfWork, 
                                    IMapper mapper,
                                    IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<Result<PostViewModel>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                return Result.Failure<PostViewModel>(postIdResult.Error);
            }

            var getPostResult = await _unitOfWork.Posts.GetPostByIdAsync(postIdResult.Value);
            if (getPostResult.IsFailure)
            {
                return Result.Failure<PostViewModel>("Post not found or user is not authorized.");
            }
            var post = getPostResult.Value;

            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<PostViewModel>("Invalid user ID.");
            }

            var userResult = await _userService.GetUserByIdAsync(userIdResult.Value);
            if (userResult.IsFailure)
            {
                return Result.Failure<PostViewModel>("User not found.");
            }

            var mappingContext = new 
            { 
                User = userResult.Value 
            };
            var postViewModel = _mapper.Map<PostViewModel>(post, opts => opts.Items["User"] = userResult.Value);

            var isPostLikedResult = await _unitOfWork.Posts
                .IsPostLikedByUserAsync(post.Id, userIdResult.Value);
            if(isPostLikedResult.IsFailure)
            {
                return Result.Failure<PostViewModel>(isPostLikedResult.Error);
            }

            postViewModel.LikedByUser = isPostLikedResult.Value;

            return Result.Success(postViewModel);
        }
    }
}
