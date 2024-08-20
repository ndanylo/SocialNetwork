using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;

namespace Posts.Application.Queries.GetPostComments
{
    public class GetCommentsQuery : IRequest<Result<List<CommentViewModel>>>
    {
        public Guid PostId { get; set; }
    }
}