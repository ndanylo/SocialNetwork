using MediatR;
using AutoMapper;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;
using Posts.Domain.Entities;
using Posts.Application.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Posts.Application.Commands.CreatePost
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<PostViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUserService _userService;
        private readonly ILogger<CreatePostCommandHandler> _logger;

        public CreatePostCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImageService imageService,
            ILogger<CreatePostCommandHandler> logger,
            IUserService userService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
            _userService = userService;
        }

        public async Task<Result<PostViewModel>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<PostViewModel>("Invalid user ID.");
            }
            var userId = userIdResult.Value;

            var userResult = await _userService.GetUserByIdAsync(userId);
            if (userResult.IsFailure)
            {
                return Result.Failure<PostViewModel>($"Error fetching user data: {userResult.Error}");
            }
            var user = userResult.Value;

            var contentResult = PostContent.Create(request.Content);
            if (contentResult.IsFailure)
            {
                return Result.Failure<PostViewModel>($"Error: {contentResult.Error}");
            }
            var postContent = contentResult.Value;

            PhotoUrl photoUrl = PhotoUrl.Create("nullPhoto").Value;
            if (request.Image != null)
            {
                photoUrl = await _imageService.SaveImageAsync(request.Image);
            }

            var postId = PostId.Create(Guid.NewGuid()).Value;
            var postResult = Post.Create(postId, userId, postContent, photoUrl);
            if (postResult.IsFailure)
            {
                return Result.Failure<PostViewModel>($"Error: {postResult.Error}");
            }
            var post = postResult.Value;

            var addPostResult = await _unitOfWork.Posts.AddPostAsync(post);
            if(addPostResult.IsFailure)
            {
                return Result.Failure<PostViewModel>($"Error: {addPostResult.Error}");
            }

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<PostViewModel>($"Error: {saveChangesResult.Error}");
            }

            var postViewModel = _mapper.Map<Post, PostViewModel>(post, opts =>
            {
                opts.Items["User"] = user;
            });

            postViewModel.LikesCount = 0;
            postViewModel.LikedByUser = false;

            return Result.Success(postViewModel);
        }
    }
}
