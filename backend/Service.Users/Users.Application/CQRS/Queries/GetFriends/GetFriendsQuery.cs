using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Users.Application.Queries.GetFriends
{
    public class GetFriendsQuery : IRequest<Result<List<UserViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}