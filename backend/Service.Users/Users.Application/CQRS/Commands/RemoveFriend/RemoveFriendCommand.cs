using MediatR;
using CSharpFunctionalExtensions;

namespace Users.Application.Commands.RemoveFriend
{
    public class RemoveFriendCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
    }
}