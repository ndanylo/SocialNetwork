using MediatR;
using CSharpFunctionalExtensions;

namespace Posts.Application.Commands.UnlikePost
{
    public class UnlikePostCommand : IRequest<Result<Unit>>
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}