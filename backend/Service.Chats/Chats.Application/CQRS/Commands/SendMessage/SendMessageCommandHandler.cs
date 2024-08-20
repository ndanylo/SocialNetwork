using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CSharpFunctionalExtensions;
using Chats.Domain.Abstractions;
using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using Chats.Application.ViewModels;
using Chats.Application.Services.Abstractions;

namespace Chats.Application.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<MessageViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public SendMessageCommandHandler(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IMapper mapper,
            IUserService userService)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Result<MessageViewModel>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var senderIdResult = UserId.Create(request.SenderId);
            if (senderIdResult.IsFailure)
            {
                return Result.Failure<MessageViewModel>("Invalid sender ID.");
            }

            var chatRoomIdResult = ChatId.Create(request.ChatId);
            if (chatRoomIdResult.IsFailure)
            {
                return Result.Failure<MessageViewModel>("Invalid chat room ID.");
            }

            var chatRoomResult = await _unitOfWork.Chats.GetChatRoomAsync(chatRoomIdResult.Value, senderIdResult.Value);
            if (chatRoomResult.IsFailure || chatRoomResult.Value == null)
            {
                return Result.Failure<MessageViewModel>("Chat room not found.");
            }

            var chatRoom = chatRoomResult.Value;
            var userIds = chatRoom.Users.Select(u => u.UserId.Id).Distinct().ToList();
            var userResults = await _userService.GetUserListByIdAsync(userIds);
            if (userResults.IsFailure)
            {
                return Result.Failure<MessageViewModel>("Failed to retrieve user details.");
            }
            var users = userResults.Value.ToDictionary(u => u.Id);

            var userSender = chatRoom.Users.FirstOrDefault(u => u.UserId == senderIdResult.Value);
            if (userSender == null)
            {
                return Result.Failure<MessageViewModel>("User not found in chat room.");
            }

            var messageContentResult = MessageContent.Create(request.Content);
            if (messageContentResult.IsFailure)
            {
                return Result.Failure<MessageViewModel>($"Message content cannot be created: {messageContentResult.Error}");
            }

            var messageCreationResult = Message.Create(
                MessageId.Create(Guid.NewGuid()).Value,
                userSender.Id,
                chatRoom.Id,
                chatRoom,
                messageContentResult.Value
            );

            if (messageCreationResult.IsFailure)
            {
                return Result.Failure<MessageViewModel>($"Message cannot be created: {messageCreationResult.Error}");
            }

            var message = messageCreationResult.Value;

            var result = await _unitOfWork.Messages.SendMessageAsync(message);
            if (result.IsFailure)
            {
                return Result.Failure<MessageViewModel>($"Message cannot be sent: {result.Error}");
            }

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<MessageViewModel>(saveChangesResult.Error);
            }

            var messageViewModel = _mapper.Map<MessageViewModel>(message, opts =>
            {
                opts.Items["Users"] = users;
                opts.Items["CurrentUserId"] = senderIdResult.Value;
            });

            foreach (var user in chatRoom.Users)
            {
                if ((Guid)user.UserId != (Guid)senderIdResult.Value)
                {
                    await _notificationService.ReceiveMessageAsync(user.UserId, messageViewModel);
                }
            }

            return Result.Success(messageViewModel);
        }
    }

}
