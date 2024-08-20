using MediatR;
using Chats.Domain.Abstractions;
using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Chats.Application.Commands.LeaveChat
{
    public class LeaveChatCommandHandler : IRequestHandler<LeaveChatCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(LeaveChatCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.Create(request.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<bool>("Invalid user ID.");
            }

            var chatRoomIdResult = ChatId.Create(request.ChatRoomId);
            if (chatRoomIdResult.IsFailure)
            {
                return Result.Failure<bool>("Invalid chat room ID.");
            }

            var result = await _unitOfWork.Chats.LeaveChatAsync(chatRoomIdResult.Value, userIdResult.Value);
            if (result.IsFailure)
            {
                return Result.Failure<bool>(result.Error);
            }
            
            var saveChangesResult = await _unitOfWork.SaveChangesAsync();
            if(saveChangesResult.IsFailure)
            {
                return Result.Failure<bool>(saveChangesResult.Error);
            }
            return Result.Success(true);
        }
    }
}
