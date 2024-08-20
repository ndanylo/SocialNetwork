using MediatR;
using Microsoft.Extensions.Logging;
using Chats.Domain.Abstractions;
using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using Chats.Application.Services.Abstractions;

namespace Chats.Application.Commands.ReadChatMessages
{
    public class ReadChatMessagesCommandHandler : IRequestHandler<ReadChatCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public ReadChatMessagesCommandHandler(
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<Result<Unit>> Handle(ReadChatCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid user ID.");
            }

            var chatRoomIdResult = ChatId.Create(request.ChatRoomId);
            if (chatRoomIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid chat room ID.");
            }

            List<Message> unreadMessages;

            try
            {
                var result = await _unitOfWork.Messages.ReadChatMessagesAsync(chatRoomIdResult.Value, userIdResult.Value);
                if (result.IsFailure)
                {
                    return Result.Failure<Unit>(result.Error);
                }
                unreadMessages = result.Value;

                await _unitOfWork.SaveChangesAsync();
                var chatRoomResult = await _unitOfWork.Chats.GetChatRoomAsync(chatRoomIdResult.Value, userIdResult.Value);
                if (chatRoomResult.IsFailure)
                {
                    return Result.Failure<Unit>(chatRoomResult.Error);
                }
                var chatRoom = chatRoomResult.Value;
                var otherUsers = chatRoom.Users
                    .Where(u => u.UserId != userIdResult.Value)
                    .Select(u => u.UserId)
                    .ToList();

                foreach (var userId in otherUsers)
                {
                    var userResult = await _userService.GetUserByIdAsync(userId.Id);
                    if (userResult.IsFailure)
                    {
                        return Result.Failure<Unit>("Failed to retrieve user details");
                    }
                    var userViewModel = userResult.Value;

                    foreach (var message in unreadMessages)
                    {
                        await _notificationService.SetMessageReadAsync(request.ChatRoomId, message.Id, userViewModel);
                    }
                }

                var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                if(saveChangesResult.IsFailure)
                {
                    return Result.Failure<Unit>(saveChangesResult.Error);
                }
                return Result.Success(Unit.Value);
            }
            catch
            {
                return Result.Failure<Unit>("Error occurred while reading chat messages.");
            }
        }
    }
}
