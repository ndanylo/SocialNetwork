using MediatR;
using CSharpFunctionalExtensions;
using Chats.Application.ViewModels;

namespace Chats.Application.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<Result<MessageViewModel>>
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
    }
}