using MediatR;
using CSharpFunctionalExtensions;

namespace Users.Application.Commands.DeclineFriendRequest
{
    public class DeclineFriendRequestCommand : IRequest<Result<Unit>>
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}