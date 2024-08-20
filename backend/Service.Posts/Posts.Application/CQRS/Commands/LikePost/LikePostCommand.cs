using MediatR;
using CSharpFunctionalExtensions;

namespace Posts.Application.Commands.LikePost
{
    public class LikePostCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }
}