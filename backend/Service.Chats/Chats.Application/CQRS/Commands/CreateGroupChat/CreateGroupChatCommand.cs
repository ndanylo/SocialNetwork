using MediatR;
using CSharpFunctionalExtensions;
using Chats.Application.ViewModels;

namespace Chats.Application.Commands.CreateGroupChat
{
    public class CreateGroupChatCommand : IRequest<Result<ChatViewModel>>
    {
        public string Name { get; set; } = string.Empty;
        public List<Guid> UserIds { get; set; } = new List<Guid>();
        public Guid CreatorId { get; set; }
    }
}