using FluentAssertions;
using Moq;
using Users.Application.Commands.DeclineFriendRequest;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using MediatR;
using Posts.Application.Services.Abstractions;

namespace Users.Application.Tests
{
    public class DeclineFriendRequestCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly DeclineFriendRequestCommandHandler _handler;

        public DeclineFriendRequestCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _notificationServiceMock = new Mock<INotificationService>();
            _handler = new DeclineFriendRequestCommandHandler(_notificationServiceMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeclineFriendRequest_WhenValid()
        {
            // Arrange
            var senderIdResult = UserId.Create(Guid.NewGuid());
            var receiverIdResult = UserId.Create(Guid.NewGuid());
            var friendRequestIdResult = FriendRequestId.Create(Guid.NewGuid());

            if (senderIdResult.IsFailure || receiverIdResult.IsFailure || friendRequestIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var senderId = senderIdResult.Value;
            var receiverId = receiverIdResult.Value;
            var friendRequestId = friendRequestIdResult.Value;

            var sender = User.Create(senderId, "sender@example.com", "senderUser", "Sender", "User", "SenderCity").Value;
            var receiver = User.Create(receiverId, "receiver@example.com", "receiverUser", "Receiver", "User", "ReceiverCity").Value;

            var friendRequest = FriendRequest.Create(friendRequestId, senderId, receiverId, sender, receiver).Value;

            _unitOfWorkMock.Setup(u => u.FriendRequests.GetFriendRequestBySenderAndReceiverIdsAsync(senderId, receiverId))
                .ReturnsAsync(friendRequest);

            _unitOfWorkMock.Setup(u => u.FriendRequests.RemoveFriendRequest(friendRequest))
                .Returns(Result.Success());

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.FromResult(Result.Success()));

            _notificationServiceMock.Setup(s => s.FriendRequestAccepted(receiverId, senderId))
                .ReturnsAsync(Result.Success(Unit.Value));

            var command = new DeclineFriendRequestCommand
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
            _notificationServiceMock.Verify(s => s.FriendRequestAccepted(receiverId, senderId), Times.Once);
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

            var command = new DeclineFriendRequestCommand
            {
                SenderId = senderId.Id,
                ReceiverId = receiverId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("You are not authorized to decline this friend request.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUnauthorized()
        {
            // Arrange
            var senderIdResult = UserId.Create(Guid.NewGuid());
            var receiverIdResult = UserId.Create(Guid.NewGuid());
            var friendRequestIdResult = FriendRequestId.Create(Guid.NewGuid());

            if (senderIdResult.IsFailure || receiverIdResult.IsFailure || friendRequestIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var senderId = senderIdResult.Value;
            var receiverId = receiverIdResult.Value;
            var friendRequestId = friendRequestIdResult.Value;

            var sender = User.Create(senderId, "sender@example.com", "senderUser", "Sender", "User", "SenderCity").Value;
            var receiver = User.Create(receiverId, "receiver@example.com", "receiverUser", "Receiver", "User", "ReceiverCity").Value;

            var friendRequest = FriendRequest.Create(friendRequestId, senderId, UserId.Create(Guid.NewGuid()).Value, sender, receiver).Value;

            _unitOfWorkMock.Setup(u => u.FriendRequests.GetFriendRequestBySenderAndReceiverIdsAsync(senderId, receiverId))
                .ReturnsAsync(friendRequest);

            var command = new DeclineFriendRequestCommand
            {
                SenderId = senderId.Id,
                ReceiverId = receiverId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("You are not authorized to decline this friend request.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNotificationFails()
        {
            // Arrange
            var senderIdResult = UserId.Create(Guid.NewGuid());
            var receiverIdResult = UserId.Create(Guid.NewGuid());
            var friendRequestIdResult = FriendRequestId.Create(Guid.NewGuid());

            if (senderIdResult.IsFailure || receiverIdResult.IsFailure || friendRequestIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var senderId = senderIdResult.Value;
            var receiverId = receiverIdResult.Value;
            var friendRequestId = friendRequestIdResult.Value;

            var sender = User.Create(senderId, "sender@example.com", "senderUser", "Sender", "User", "SenderCity").Value;
            var receiver = User.Create(receiverId, "receiver@example.com", "receiverUser", "Receiver", "User", "ReceiverCity").Value;

            var friendRequest = FriendRequest.Create(friendRequestId, senderId, receiverId, sender, receiver).Value;

            _unitOfWorkMock.Setup(u => u.FriendRequests.GetFriendRequestBySenderAndReceiverIdsAsync(senderId, receiverId))
                .ReturnsAsync(friendRequest);

            _unitOfWorkMock.Setup(u => u.FriendRequests.RemoveFriendRequest(friendRequest))
                .Returns(Result.Success());

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.FromResult(Result.Success()));

            _notificationServiceMock.Setup(s => s.FriendRequestAccepted(receiverId, senderId))
                .ReturnsAsync(Result.Failure<Unit>("Failed to send notification"));

            var command = new DeclineFriendRequestCommand
            {
                SenderId = senderId.Id,
                ReceiverId = receiverId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Failed to send notification: Failed to send notification");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInvalidSenderId()
        {
            // Arrange
            var receiverIdResult = UserId.Create(Guid.NewGuid());

            if (receiverIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var receiverId = receiverIdResult.Value;

            var command = new DeclineFriendRequestCommand
            {
                SenderId = Guid.Empty,
                ReceiverId = receiverId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Invalid sender ID.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInvalidReceiverId()
        {
            // Arrange
            var senderIdResult = UserId.Create(Guid.NewGuid());

            if (senderIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var senderId = senderIdResult.Value;

            var command = new DeclineFriendRequestCommand
            {
                SenderId = senderId.Id,
                ReceiverId = Guid.Empty
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Invalid receiver ID.");
        }
    }
}
