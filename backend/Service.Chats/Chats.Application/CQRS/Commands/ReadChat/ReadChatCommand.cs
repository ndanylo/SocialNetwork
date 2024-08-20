using MediatR;
using CSharpFunctionalExtensions;

namespace Chats.Application.Commands.ReadChatMessages
{
    public class ReadChatCommand : IRequest<Result<Unit>>
    {
        public Guid ChatRoomId { get; set; }
        public Guid UserId { get; set; }
    }
}
