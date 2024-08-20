using MediatR;
using CSharpFunctionalExtensions;

namespace OnlineChat.Application.FriendRequests.Commands.CancelFriendRequest
{
    public class CancelFriendRequestCommand : IRequest<Result<Unit>>
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
    }
}