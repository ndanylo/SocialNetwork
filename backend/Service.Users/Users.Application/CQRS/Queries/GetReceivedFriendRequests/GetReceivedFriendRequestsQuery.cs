using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Users.Application.Queries.GetReceivedFriendRequests
{
    public class GetReceivedFriendRequestsQuery : IRequest<Result<List<UserViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}