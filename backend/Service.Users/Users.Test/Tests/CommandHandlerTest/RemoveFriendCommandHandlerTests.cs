using FluentAssertions;
using Moq;
using Users.Application.Commands.RemoveFriend;
using Users.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Posts.Application.Services.Abstractions;
using OnlineChat.Application.Users.Commands.RemoveFriend;
using MediatR;

namespace Users.Application.Tests.Commands
{
    public class RemoveFriendCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly RemoveFriendCommandHandler _handler;

        public RemoveFriendCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _notificationServiceMock = new Mock<INotificationService>();
            _handler = new RemoveFriendCommandHandler(_notificationServiceMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldRemoveFriend_WhenValid()
        {
            // Arrange
            var userIdResult = UserId.Create(Guid.NewGuid());
            var friendIdResult = UserId.Create(Guid.NewGuid());

            if (userIdResult.IsFailure || friendIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var userId = userIdResult.Value;
            var friendId = friendIdResult.Value;

            _unitOfWorkMock.Setup(u => u.Users.RemoveFriendAsync(userId, friendId))
                .Returns(Task.FromResult(Result.Success()));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.FromResult(Result.Success()));

            _notificationServiceMock.Setup(s => s.FriendRemoved(userId, friendId))
                .ReturnsAsync(Result.Success(Unit.Value));

            var command = new RemoveFriendCommand
            {
                UserId = userId.Id,
                FriendId = friendId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.Users.RemoveFriendAsync(userId, friendId), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(s => s.FriendRemoved(userId, friendId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdInvalid()
        {
            // Arrange
            var invalidUserId = Guid.Empty;
            var friendId = Guid.NewGuid();
            var friendIdResult = UserId.Create(friendId);

            if (friendIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value object for test.");
            }

            var command = new RemoveFriendCommand
            {
                UserId = invalidUserId,
                FriendId = friendId
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Invalid user ID.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenFriendIdInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var invalidFriendId = Guid.Empty;
            var userIdResult = UserId.Create(userId);

            if (userIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value object for test.");
            }

            var command = new RemoveFriendCommand
            {
                UserId = userId,
                FriendId = invalidFriendId
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Invalid friend ID.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRemoveFriendThrowsException()
        {
            // Arrange
            var userIdResult = UserId.Create(Guid.NewGuid());
            var friendIdResult = UserId.Create(Guid.NewGuid());

            if (userIdResult.IsFailure || friendIdResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to create value objects for test.");
            }

            var userId = userIdResult.Value;
            var friendId = friendIdResult.Value;

            _unitOfWorkMock.Setup(u => u.Users.RemoveFriendAsync(userId, friendId))
                .ReturnsAsync(Result.Failure("Database error"));

            var command = new RemoveFriendCommand
            {
                UserId = userId.Id,
                FriendId = friendId.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Database error");
        }
    }
}
