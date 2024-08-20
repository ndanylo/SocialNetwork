using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace OnlineChat.Application.FriendRequests.Queries.GetSentFriendRequests
{
    public class GetSentFriendRequestsQuery : IRequest<Result<List<UserViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}