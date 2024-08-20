using MediatR;
using CSharpFunctionalExtensions;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;
using Posts.Application.Services.Abstractions;
using Posts.Domain.Entities;
using Posts.Application.Services.Contracts;

namespace Posts.Application.Commands.LikePost
{
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public LikePostCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result<Unit>> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid user ID.");
            }

            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                return Result.Failure<Unit>(postIdResult.Error);
            }

            var getPostResult = await _unitOfWork.Posts.GetPostByIdAsync(postIdResult.Value);
            if (getPostResult.IsFailure)
            {
                return Result.Failure<Unit>($"Post with ID {request.PostId} not found.");
            }
            var post = getPostResult.Value;

            if (post.UserId == userIdResult.Value)
            {
                return Result.Failure<Unit>($"You cannot like your own post.");
            }

            if (post.Likes.Any(l => l.UserId == userIdResult.Value))
            {
                return Result.Failure<Unit>("Post already liked by this user.");
            }

            var likeResult = Like.Create(LikeId.Create(Guid.NewGuid()).Value,
                                        userIdResult.Value,
                                        postIdResult.Value);

            if (likeResult.IsFailure)
            {
                return Result.Failure<Unit>("Error while liking the post.");
            }

            var addLikeAsync = await _unitOfWork.Likes.AddLikeAsync(likeResult.Value);
            if(addLikeAsync.IsFailure)
            {
                return Result.Failure<Unit>("Error while liking the post.");
            }

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<Unit>($"Error: {saveChangesResult.Error}");
            }

            await _notificationService
                .CreateNotificationAsync(post.UserId, post.Id, $"User {userIdResult.Value} liked your post.", NotificationType.Like);

            return Result.Success(Unit.Value);
        }
    }
}
