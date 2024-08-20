using MediatR;
using AutoMapper;
using CSharpFunctionalExtensions;
using Chats.Domain.Abstractions;
using Chats.Application.ViewModels;
using Chats.Domain.ValueObjects;
using Chats.Application.Services.Abstractions;

namespace Chats.Application.Queries.GetUserChats
{
    public class GetUserChatsQueryHandler : IRequestHandler<GetUserChatsQuery, Result<List<ChatViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetUserChatsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Result<List<ChatViewModel>>> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<List<ChatViewModel>>("Invalid user ID.");
            }

            var getChatsResult = await _unitOfWork.Chats.GetUserChatsAsync(userIdResult.Value);
            if(getChatsResult.IsFailure)
            {
                return Result.Failure<List<ChatViewModel>>(getChatsResult.Error);
            }
            
            var chats = getChatsResult.Value;
            var userIds = new HashSet<Guid>();
            foreach (var chat in chats)
            {
                foreach (var user in chat.Users)
                {
                    userIds.Add(user.UserId);
                }
            }

            var userDetailsResult = await _userService.GetUserListByIdAsync(userIds);
            if (userDetailsResult.IsFailure)
            {
                return Result.Failure<List<ChatViewModel>>(userDetailsResult.Error);
            }

            var userDictionary = userDetailsResult.Value.ToDictionary(u => u.Id, u => u);
            var chatViewModels = _mapper.Map<List<ChatViewModel>>(chats, opt =>
            {
                opt.Items["CurrentUserId"] = (Guid)userIdResult.Value;
                opt.Items["Users"] = userDictionary;
            });

            foreach (var chatViewModel in chatViewModels)
            {
                var chatRoomIdResult = ChatId.Create(chatViewModel.Id);
                if (chatRoomIdResult.IsFailure)
                {
                    return Result.Failure<List<ChatViewModel>>($"Invalid ChatRoomId: {chatRoomIdResult.Error}");
                }
                var unreadMessageCountResult = await _unitOfWork.Messages
                    .GetUnreadMessageCountForUserAsync(userIdResult.Value, chatRoomIdResult.Value);

                if (unreadMessageCountResult.IsSuccess && unreadMessageCountResult.Value is int unreadCount)
                {
                    chatViewModel.UnreadMessagesCount = unreadCount;
                }
                else
                {
                    chatViewModel.UnreadMessagesCount = 0;
                }
                chatViewModel.LastMessage = chatViewModel.Messages?.LastOrDefault() ?? new MessageViewModel();

                chatViewModel.Messages = new List<MessageViewModel>();
            }

            return Result.Success(chatViewModels);
        }
    }
}