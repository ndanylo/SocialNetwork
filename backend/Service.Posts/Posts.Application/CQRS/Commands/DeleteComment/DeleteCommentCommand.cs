using MediatR;
using CSharpFunctionalExtensions;

namespace OnlineChat.Application.Comments.Commands.DeleteComment
{
    public class DeleteCommentCommand : IRequest<Result<Unit>>
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
    }
}