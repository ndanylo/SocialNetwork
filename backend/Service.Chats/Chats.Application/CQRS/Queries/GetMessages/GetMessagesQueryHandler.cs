using AutoMapper;
using Chats.Application.Services.Abstractions;
using Chats.Application.ViewModels;
using Chats.Domain.Abstractions;
using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chats.Application.Queries.GetMessages
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, Result<List<MessageViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMessagesQueryHandler> _logger;

        public GetMessagesQueryHandler(IUnitOfWork unitOfWork,
                                       IUserService userService,
                                       IMapper mapper,
                                       ILogger<GetMessagesQueryHandler> logger,
                                       INotificationService notificationService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Result<List<MessageViewModel>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var chatIdResult = ChatId.Create(request.ChatRoomId);
                if (chatIdResult.IsFailure)
                {
                    return Result.Failure<List<MessageViewModel>>(chatIdResult.Error);
                }

                var userIdResult = UserId.Create(request.UserId);
                if (userIdResult.IsFailure)
                {
                    return Result.Failure<List<MessageViewModel>>(userIdResult.Error);
                }

                var chatId = chatIdResult.Value;
                var userId = userIdResult.Value;

                var getMessagesResult = await _unitOfWork.Messages.GetMessagesAsync(chatId, request.AmountOfMessage, request.Count, userId);
                if (getMessagesResult.IsFailure)
                {
                    return Result.Failure<List<MessageViewModel>>(getMessagesResult.Error);
                }

                var messages = getMessagesResult.Value;
                var uniqueUserIds = messages.Select(m => (Guid)m.ChatUserId.UserId).ToHashSet();

                var userDetailsResult = await _userService.GetUserListByIdAsync(uniqueUserIds);
                if (userDetailsResult.IsFailure)
                {
                    return Result.Failure<List<MessageViewModel>>(userDetailsResult.Error);
                }

                var userDetailsDictionary = userDetailsResult.Value.ToDictionary(u => u.Id, u => u);

                var messageViewModels = _mapper.Map<List<MessageViewModel>>(messages, opts =>
                {
                    opts.Items["Users"] = userDetailsDictionary;
                    opts.Items["CurrentUserId"] = request.UserId;
                });

                if (request.Count == 0)
                {
                    var notificationResult = await _notificationService.ReadChatAsync(request.UserId, request.ChatRoomId);
                    if (notificationResult.IsFailure)
                    {
                        _logger.LogError("Failed to read chat notifications: {Error}", notificationResult.Error);
                        return Result.Failure<List<MessageViewModel>>(notificationResult.Error);
                    }
                }

                return Result.Success(messageViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetMessagesQuery");
                return Result.Failure<List<MessageViewModel>>(ex.Message);
            }
        }
    }
}
