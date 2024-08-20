using Chats.Application.Commands.CreateGroupChat;
using Chats.Application.Commands.LeaveChat;
using Chats.Application.Commands.ReadChatMessages;
using Chats.Application.Commands.SendMessage;
using Chats.Application.Queries.GetChatByUser;
using Chats.Application.Queries.GetMessages;
using Chats.Application.Queries.GetUserChats;

namespace Chats.WebApi.DI
{
    public static class MediatRService
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateGroupChatCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(LeaveChatCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(ReadChatCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(SendMessageCommand).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetChatByUserQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetMessagesQuery).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetUserChatsQuery).Assembly);
            });

            return services;
        }
    }
}
