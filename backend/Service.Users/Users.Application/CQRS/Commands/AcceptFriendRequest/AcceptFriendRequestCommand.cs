using MediatR;
using CSharpFunctionalExtensions;

namespace Users.Application.Commands.AcceptFriendRequest
{
    public class AcceptFriendRequestCommand : IRequest<Result<Unit>>
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
    }
}