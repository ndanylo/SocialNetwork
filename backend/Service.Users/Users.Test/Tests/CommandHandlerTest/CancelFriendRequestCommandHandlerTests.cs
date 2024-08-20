using FluentAssertions;
using Moq;
using OnlineChat.Application.FriendRequests.Commands.CancelFriendRequest;
using Users.Domain.ValueObjects;
using Users.Domain.Entities;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Posts.Application.Services.Abstractions;
using MediatR;

namespace OnlineChat.Application.Tests.FriendRequests.Commands
{
    public class CancelFriendRequestCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly CancelFriendRequestCommandHandler _handler;

        public CancelFriendRequestCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _notificationServiceMock = new Mock<INotificationService>();
            _handler = new CancelFriendRequestCommandHandler(_notificationServiceMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCancelFriendRequest_WhenValid()
        {
            // Arrange
            var senderIdResult = UserId.Create(Guid.NewGuid());
            var receiverIdResult = UserId.Create(Guid.NewGuid());

            if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var senderId = senderIdResult.Value;
            var receiverId = receiverIdResult.Value;

            var friendRequest = FriendRequest
                .Create(FriendRequestId.Create(Guid.NewGuid()).Value, senderId, receiverId,
                                            User.Create(senderId, "sender@example.com", "senderUser", "Sender", "User", "SenderCity").Value,
                                            User.Create(receiverId, "receiver@example.com", "receiverUser", "Receiver", "User", "ReceiverCity").Value).Value;

            _unitOfWorkMock.Setup(u => u.FriendRequests.GetFriendRequestBySenderAndReceiverIdsAsync(senderId, receiverId))
                .ReturnsAsync(friendRequest);

            _unitOfWorkMock.Setup(u => u.FriendRequests.RemoveFriendRequest(friendRequest))
                .Returns(Result.Success());

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.FromResult(Result.Success()));

            _notificationServiceMock.Setup(s => s.FriendRequestCancelled(receiverId, senderId))
                .ReturnsAsync(Result.Success(Unit.Value));

            var command = new CancelFriendRequestCommand
            {
                SenderId = senderId.Id,
                ReceiverId = receiverId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.FriendRequests.RemoveFriendRequest(friendRequest), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(s => s.FriendRequestCancelled(receiverId, senderId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenFriendRequestNotFound()
        {
            // Arrange
            var senderIdResult = UserId.Create(Guid.NewGuid());
            var receiverIdResult = UserId.Create(Guid.NewGuid());

            if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var senderId = senderIdResult.Value;
            var receiverId = receiverIdResult.Value;

            _unitOfWorkMock.Setup(u => u.FriendRequests.GetFriendRequestBySenderAndReceiverIdsAsync(senderId, receiverId))
                .ReturnsAsync(FriendRequest.Default);

            var command = new CancelFriendRequestCommand
            {
                SenderId = senderId.Id,
                ReceiverId = receiverId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("You are not authorized to cancel this friend request.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUnauthorized()
        {
            // Arrange
            var senderIdResult = UserId.Create(Guid.NewGuid());
            var receiverIdResult = UserId.Create(Guid.NewGuid());

            if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var senderId = senderIdResult.Value;
            var receiverId = receiverIdResult.Value;

            var friendRequest = FriendRequest
            .Create(FriendRequestId.Create(Guid.NewGuid()).Value, UserId.Create(Guid.NewGuid()).Value, receiverId,
                                        User.Create(senderId, "sender@example.com", "senderUser", "Sender", "User", "SenderCity").Value,
                                        User.Create(receiverId, "receiver@example.com", "receiverUser", "Receiver", "User", "ReceiverCity").Value).Value;

            _unitOfWorkMock.Setup(u => u.FriendRequests.GetFriendRequestBySenderAndReceiverIdsAsync(senderId, receiverId))
                .ReturnsAsync(friendRequest);

            var command = new CancelFriendRequestCommand
            {
                SenderId = senderId.Id,
                ReceiverId = receiverId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("You are not authorized to cancel this friend request.");
        }
    }
}
