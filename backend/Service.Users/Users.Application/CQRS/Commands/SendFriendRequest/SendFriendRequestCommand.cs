using MediatR;
using CSharpFunctionalExtensions;

namespace Users.Application.Commands.SendFriendRequest
{
    public class SendFriendRequestCommand : IRequest<Result<Unit>>
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}