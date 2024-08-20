using AutoMapper;
using MediatR;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Chats.Domain.ValueObjects;
using Chats.Domain.Abstractions;
using Chats.Application.ViewModels;
using Chats.Application.Services.Abstractions;

namespace Chats.Application.Queries.GetChatByUser
{
    public class GetChatByUserQueryHandler : IRequestHandler<GetChatByUserQuery, Result<ChatViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetChatByUserQueryHandler> _logger;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public GetChatByUserQueryHandler(IUnitOfWork unitOfWork,
                                         ILogger<GetChatByUserQueryHandler> logger,
                                         INotificationService notificationService,
                                         IMapper mapper,
                                         IUserService userService)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
        }

        public async Task<Result<ChatViewModel>> Handle(GetChatByUserQuery request, CancellationToken cancellationToken)
        {
            var senderIdResult = UserId.Create(request.UserSenderId);
            if (senderIdResult.IsFailure)
            {
                return Result.Failure<ChatViewModel>("Invalid sender ID.");
            }

            var receiverIdResult = UserId.Create(request.UserReceiverId);
            if (receiverIdResult.IsFailure)
            {
                return Result.Failure<ChatViewModel>("Invalid receiver ID.");
            }

            var chatRoomResult = await _unitOfWork.Chats.GetPrivateChatByUsersAsync(senderIdResult.Value, receiverIdResult.Value);
            if (chatRoomResult.IsFailure)
            {
                var chatRoomNameResult = ChatRoomName.Create($"{senderIdResult.Value}, {receiverIdResult.Value}");
                if (chatRoomNameResult.IsSuccess && chatRoomNameResult.Value is ChatRoomName chatRoomName)
                {
                    var createChatResult = _unitOfWork.Chats.CreatePrivateChat(senderIdResult.Value, receiverIdResult.Value, chatRoomName);
                    if(createChatResult.IsFailure)
                    {
                        return Result.Failure<ChatViewModel>(createChatResult.Error);
                    }

                    var createdChat = createChatResult.Value;
                    if (createdChat == null)
                    {
                        return Result.Failure<ChatViewModel>("Failed to create chat.");
                    }

                    chatRoomResult = Result.Success(createdChat);
                }
                else
                {
                    return Result.Failure<ChatViewModel>("Chat cannot be created.");
                }
            }

            var chatRoom = chatRoomResult.Value;

            var unreadMessagesResult = await _unitOfWork.Messages.GetUnreadMessageCountForUserAsync(senderIdResult.Value, chatRoom.Id);
            if (unreadMessagesResult.IsFailure)
            {
                return Result.Failure<ChatViewModel>("Error occurred while retrieving unread messages.");
            }

            var userIds = chatRoom.Users.Select(u => (Guid)u.UserId).Distinct().ToList();
            var usersResult = await _userService.GetUserListByIdAsync(userIds);
            if (usersResult.IsFailure)
            {
                return Result.Failure<ChatViewModel>("Error occurred while retrieving user details.");
            }

            var users = usersResult.Value.ToDictionary(u => u.Id, u => u);
            var chatViewModel = _mapper.Map<ChatViewModel>(chatRoom, opt =>
            {
                opt.Items["CurrentUserId"] = senderIdResult.Value;
                opt.Items["Users"] = users;
            });

            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<ChatViewModel>(saveChangesResult.Error);
            }
            var notificationResult = await _notificationService.ReadChatAsync(request.UserSenderId, chatRoom.Id);
            if (notificationResult.IsFailure)
            {
                _logger.LogError("Failed to send chat read notification for chat {ChatId} and user {UserId}.", chatRoom.Id, senderIdResult.Value);
            }

            return Result.Success(chatViewModel);
        }
    }
}
