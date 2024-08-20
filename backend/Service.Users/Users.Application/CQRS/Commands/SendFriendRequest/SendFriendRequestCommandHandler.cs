using MediatR;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Users.Domain.Entities;
using Posts.Application.Services.Abstractions;
using AutoMapper;
using Users.Application.ViewModels;

namespace Users.Application.Commands.SendFriendRequest
{
    public class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public SendFriendRequestCommandHandler(INotificationService notificationService,
                                            IUnitOfWork unitOfWork,
                                            IMapper mapper)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var senderIdResult = UserId.Create(request.SenderId);
            if (senderIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid sender ID.");
            }
            var receiverIdResult = UserId.Create(request.ReceiverId);
            if (receiverIdResult.IsFailure)
            {
                return Result.Failure<Unit>("Invalid receiver ID.");
            }

            var getSenderResult = await _unitOfWork.Users.GetUserByIdAsync(senderIdResult.Value);
            if(getSenderResult.IsFailure)
            {
                return Result.Failure<Unit>(getSenderResult.Error);
            }

            var getReceiverResult = await _unitOfWork.Users.GetUserByIdAsync(receiverIdResult.Value);
            if(getReceiverResult.IsFailure)
            {
                return Result.Failure<Unit>(getReceiverResult.Error);
            }
            var sender = getSenderResult.Value;
            var receiver = getReceiverResult.Value;    

            if (sender == null || receiver == null)
            {
                return Result.Failure<Unit>("Sender or receiver not found.");
            }

            var areFriendsResult = await _unitOfWork.Users.AreFriendsAsync(sender.Id, receiver.Id);
            if(areFriendsResult.IsFailure)
            {
                return Result.Failure<Unit>(areFriendsResult.Error);
            }
            var areFriends = areFriendsResult.Value;

            if (areFriends)
            {
                return Result.Failure<Unit>("Users already have friendship.");
            }

            var friendRequestExists = await _unitOfWork.FriendRequests
                .ExistsAsync(sender.Id, receiver.Id);
            if(friendRequestExists.IsFailure)
            {
                return Result.Failure<Unit>("Users already have friendship.");
            }
            var isFriendExists = friendRequestExists.Value;

            if (isFriendExists)
            {
                return Result.Failure<Unit>("Friend request already sent.");
            }

            var result = FriendRequest
                .Create(FriendRequestId.Create(Guid.NewGuid()).Value, sender.Id, receiver.Id, sender, receiver);

            if (result.IsFailure)
            {
                return Result.Failure<Unit>("Error while friend request creating");
            }

            var friendRequest = result.Value;
            try
            {
                var addFriendResult = await _unitOfWork.FriendRequests.AddFriendRequestAsync(friendRequest);
                if(addFriendResult.IsFailure)
                {
                    return Result.Failure<Unit>(addFriendResult.Error);
                }

                var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                if(saveChangesResult.IsFailure)
                {
                    return Result.Failure<Unit>(saveChangesResult.Error);
                }

                var senderViewModel = _mapper.Map<UserViewModel>(sender);
                var notificationResult = await _notificationService.FriendRequestReceived(receiver.Id, senderViewModel);
                if (notificationResult.IsFailure)
                {
                    return Result.Failure<Unit>($"Failed to send notification: {notificationResult.Error}");
                }

                return Result.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result.Failure<Unit>($"Error while friend request adding to database {ex.Message}");
            }
        }
    }
}