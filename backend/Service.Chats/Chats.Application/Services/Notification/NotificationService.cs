using CSharpFunctionalExtensions;
using MassTransit;
using Chats.Application.Services.Abstractions;
using MessageBus.Contracts.Requests;
using Chats.Application.ViewModels;

namespace Chats.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public NotificationService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result> ReadChatAsync(Guid userId, Guid chatRoomId)
        {
            var request = new ReadChatRequest
            {
                UserId = userId,
                ChatRoomId = chatRoomId
            };

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }

        public async Task<Result> ReceiveMessageAsync(Guid receiverId, MessageViewModel message)
        {
            var request = new SendMessageRequest
            {
                ReceiverId = receiverId,
                Message = message
            };

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }

        public async Task<Result> SetMessageReadAsync(Guid chatRoomId, Guid messageId, UserViewModel reader)
        {
            var request = new SetMessageReadRequest
            {
                ChatRoomId = chatRoomId,
                MessageId = messageId,
                Reader = reader
            };

            await _publishEndpoint.Publish(request);
            return Result.Success();
        }
    }
}
