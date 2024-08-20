using MediatR;
using CSharpFunctionalExtensions;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Commands.UnlikePost
{
    public class UnlikePostCommandHandler : IRequestHandler<UnlikePostCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnlikePostCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid user ID.");
            }

            try
            {
                var postIdResult = PostId.Create(request.PostId);
                if (postIdResult.IsFailure)
                {
                    return Result.Failure<Unit>(postIdResult.Error);
                }

                var getLikeResult = await _unitOfWork.Likes.GetLikeAsync(postIdResult.Value, userIdResult.Value);
                if(getLikeResult.IsFailure)
                {
                    return Result.Failure<Unit>(getLikeResult.Error);
                }
                var like = getLikeResult.Value;

                if (like != null)
                {
                    var removeLikeResult = _unitOfWork.Likes.RemoveLike(like);
                    if(removeLikeResult.IsFailure)
                    {
                        return Result.Failure<Unit>($"Error: {removeLikeResult.Error}");
                    }

                    var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                    if(saveChangesResult.IsFailure)
                    {
                        return Result.Failure<Unit>($"Error: {saveChangesResult.Error}");
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<Unit>($"Error while removing of like: {ex.Message}");
            }

            return Result.Success(Unit.Value);
        }
    }
}