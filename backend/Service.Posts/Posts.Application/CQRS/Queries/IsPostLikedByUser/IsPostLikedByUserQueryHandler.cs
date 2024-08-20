using MediatR;
using CSharpFunctionalExtensions;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Queries.IsPostLikedByUser
{
    public class IsPostLikedByUserQueryHandler : IRequestHandler<IsPostLikedByUserQuery, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public IsPostLikedByUserQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(IsPostLikedByUserQuery request, CancellationToken cancellationToken)
        {
            var postIdResult = PostId.Create(request.PostId);
            if (postIdResult.IsFailure)
            {
                return Result.Failure<bool>(postIdResult.Error);
            }

            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<bool>(userIdResult.Error);
            }

            var isLikedResult = await _unitOfWork.Likes.IsPostLikedByUserAsync(postIdResult.Value, userIdResult.Value);
            if(isLikedResult.IsFailure)
            {
                return Result.Failure<bool>($"Error while checking if post is liked: {isLikedResult.Error}");
            }

            return Result.Success(isLikedResult.Value);
        }
    }
}
