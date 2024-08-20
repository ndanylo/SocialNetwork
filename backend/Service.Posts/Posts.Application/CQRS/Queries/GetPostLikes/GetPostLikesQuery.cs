using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;

namespace Posts.Application.Queries.GetPostLikes
{
    public class GetPostLikesQuery : IRequest<Result<List<LikeViewModel>>>
    {
        public Guid PostId { get; set; }
    }
}