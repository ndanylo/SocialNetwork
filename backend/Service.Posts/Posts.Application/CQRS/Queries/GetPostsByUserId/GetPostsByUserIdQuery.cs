using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;

namespace Posts.Application.Queries.GetPostsByUserId
{
    public class GetPostsByUserIdQuery : IRequest<Result<List<PostViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}