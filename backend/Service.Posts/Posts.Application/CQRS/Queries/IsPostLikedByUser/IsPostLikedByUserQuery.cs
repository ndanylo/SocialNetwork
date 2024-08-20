using MediatR;
using CSharpFunctionalExtensions;

namespace Posts.Application.Queries.IsPostLikedByUser
{
    public class IsPostLikedByUserQuery : IRequest<Result<bool>>
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
