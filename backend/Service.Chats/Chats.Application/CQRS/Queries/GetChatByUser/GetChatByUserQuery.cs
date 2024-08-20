using MediatR;
using CSharpFunctionalExtensions;
using Chats.Application.ViewModels;

namespace Chats.Application.Queries.GetChatByUser
{
    public class GetChatByUserQuery : IRequest<Result<ChatViewModel>>
    {
        public Guid UserSenderId { get; set; }
        public Guid UserReceiverId { get; set; }
    }
}
