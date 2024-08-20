using MediatR;
using CSharpFunctionalExtensions;

namespace Chats.Application.Commands.LeaveChat
{
    public class LeaveChatCommand : IRequest<Result<bool>>
    {
        public Guid UserId { get; set; }
        public Guid ChatRoomId { get; set; }
    }
}
