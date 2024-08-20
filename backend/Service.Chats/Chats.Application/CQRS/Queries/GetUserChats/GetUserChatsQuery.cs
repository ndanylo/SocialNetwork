using MediatR;
using CSharpFunctionalExtensions;
using Chats.Application.ViewModels;

namespace Chats.Application.Queries.GetUserChats
{
    public class GetUserChatsQuery : IRequest<Result<List<ChatViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}
