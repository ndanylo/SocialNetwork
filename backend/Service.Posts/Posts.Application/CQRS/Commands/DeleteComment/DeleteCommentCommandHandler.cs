using MediatR;
using CSharpFunctionalExtensions;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;

namespace OnlineChat.Application.Comments.Commands.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var commentIdResult = CommentId.Create(request.CommentId);
            if (commentIdResult.IsFailure)
            {
                return Result.Failure<Unit>(commentIdResult.Error);
            }

            var getCommentResult = await _unitOfWork.Comments.GetCommentByIdAsync(commentIdResult.Value);
            if(getCommentResult.IsFailure)
            {
                return Result.Failure<Unit>(getCommentResult.Error);
            }
            var comment = getCommentResult.Value;

            if (comment == null || comment.UserId.Id != request.UserId)
            {
                return Result.Failure<Unit>("Comment not found or user is not authorized.");
            }

            var removeCommentAsync = _unitOfWork.Comments.RemoveComment(comment);
            if(removeCommentAsync.IsFailure)
            {
                return Result.Failure<Unit>(removeCommentAsync.Error);
            }

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<Unit>($"Error: {saveChangesResult.Error}");
            }

            return Result.Success(Unit.Value);
        }
    }
}