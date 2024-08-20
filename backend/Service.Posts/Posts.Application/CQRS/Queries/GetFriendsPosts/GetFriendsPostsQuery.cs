using MediatR;
using CSharpFunctionalExtensions;
using Posts.Application.ViewModels;

namespace Posts.Application.Queries.GetFriendsPosts
{
    public class GetFriendsPostsQuery : IRequest<Result<List<PostViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}