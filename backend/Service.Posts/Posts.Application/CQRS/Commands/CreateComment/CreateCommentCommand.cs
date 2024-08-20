using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;

namespace Posts.Application.Commands.CreateComment
{
    public class CreateCommentCommand : IRequest<Result<CommentViewModel>>
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}