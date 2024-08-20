using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;

namespace Posts.Application.Queries.GetPostById
{
    public class GetPostByIdQuery : IRequest<Result<PostViewModel>>
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}