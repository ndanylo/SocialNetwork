using AutoMapper;
using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;
using Posts.Application.Services.Contracts;

namespace Posts.Application.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public CreateCommentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<Result<CommentViewModel>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<CommentViewModel>("Invalid user ID.");
            }

            var userResult = await _userService.GetUserByIdAsync(userIdResult.Value);
            if (userResult.IsFailure)
            {
                return Result.Failure<CommentViewModel>("User not found.");
            }

            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                return Result.Failure<CommentViewModel>(postIdResult.Error);
            }

            var getPostResult = await _unitOfWork.Posts.GetPostByIdAsync(postIdResult.Value);
            if (getPostResult.IsFailure)
            {
                return Result.Failure<CommentViewModel>($"Post with ID {request.PostId} not found.");
            }
            var post = getPostResult.Value;

            var commentContentResult = CommentContent.Create(request.Content);
            if (commentContentResult.IsFailure)
            {
                return Result.Failure<CommentViewModel>(commentContentResult.Error);
            }

            var comment = Comment.Create(CommentId.Create(Guid.NewGuid()).Value,
                                        userIdResult.Value,
                                        postIdResult.Value,
                                        commentContentResult.Value);

            if (comment.IsFailure)
            {
                return Result.Failure<CommentViewModel>(comment.Error);
            }

            var addCommentsResult = await _unitOfWork.Comments.AddCommentAsync(comment.Value);
            if(addCommentsResult.IsFailure)
            {
                return Result.Failure<CommentViewModel>(addCommentsResult.Error);
            }

            var commentViewModel = _mapper.Map<CommentViewModel>(comment.Value, opt =>
            {
                opt.Items["User"] = userResult.Value;
            });

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<CommentViewModel>(saveChangesResult.Error);
            }

            await _notificationService.CreateNotificationAsync(post.UserId,
                                        postIdResult.Value, 
                                        $"User {userIdResult.Value} commented on your post.",
                                        NotificationType.Comment);

            return Result.Success(commentViewModel);
        }
    }
}
