using MediatR;
using AutoMapper;
using CSharpFunctionalExtensions;
using Chats.Domain.Abstractions;
using Chats.Domain.ValueObjects;
using Chats.Application.ViewModels;
using Chats.Application.Services.Abstractions;
namespace Chats.Application.Commands.CreateGroupChat
{
    public class CreateGroupChatCommandHandler : IRequestHandler<CreateGroupChatCommand, Result<ChatViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CreateGroupChatCommandHandler(IUnitOfWork unitOfWork,
                                            IMapper mapper,
                                            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Result<ChatViewModel>> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return Result.Failure<ChatViewModel>("Request cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Failure<ChatViewModel>("Chat name is required.");
            }
            if (request.UserIds == null || !request.UserIds.Any())
            {
                return Result.Failure<ChatViewModel>("At least one user ID is required.");
            }

            var usersToAdd = new List<Guid>(request.UserIds.Distinct());

            if (!usersToAdd.Contains(request.CreatorId))
            {
                usersToAdd.Add(request.CreatorId);
            }

            var userDetailsResult = await _userService.GetUserListByIdAsync(usersToAdd);
            if (userDetailsResult.IsFailure)
            {
                return Result.Failure<ChatViewModel>(userDetailsResult.Error);
            }

            var userDetails = userDetailsResult.Value;

            var chatNameResult = ChatRoomName.Create(request.Name);
            if (chatNameResult.IsFailure)
            {
                return Result.Failure<ChatViewModel>(chatNameResult.Error);
            }

            var userIds = usersToAdd.Select(id => UserId.Create(id))
                                    .Where(result => result.IsSuccess)
                                    .Select(result => result.Value)
                                    .ToList();

            if (userIds.Count != usersToAdd.Count)
            {
                return Result.Failure<ChatViewModel>("One or more user IDs are invalid.");
            }
            try
            {
                var createChatResult = _unitOfWork.Chats.CreateGroupChat(chatNameResult.Value, userIds);
                if (createChatResult.IsFailure)
                {
                    return Result.Failure<ChatViewModel>(createChatResult.Error);
                }

                var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                if(saveChangesResult.IsFailure)
                {
                    return Result.Failure<ChatViewModel>(saveChangesResult.Error);
                }

                var chat = createChatResult.Value;
                var chatViewModel = _mapper.Map<ChatViewModel>(chat, opt =>
                {
                    opt.Items["CurrentUserId"] = request.CreatorId;
                    opt.Items["Users"] = userDetails;
                });
                return Result.Success(chatViewModel);
            }
            catch
            {
                return Result.Failure<ChatViewModel>("Error occurred while creating group chat.");
            }
        }
    }
}