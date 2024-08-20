using Chats.Application.ViewModels;
using CSharpFunctionalExtensions;
using MediatR;

namespace Chats.Application.Queries.GetMessages
{
    public class GetMessagesQuery : IRequest<Result<List<MessageViewModel>>>
    {
        public Guid ChatRoomId { get; set; }
        public int AmountOfMessage { get; set; }
        public int Count { get; set; }
        public Guid UserId { get; set; }
    }
}
