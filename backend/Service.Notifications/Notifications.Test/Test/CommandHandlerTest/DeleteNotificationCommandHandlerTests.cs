using Moq;
using Notifications.Application.Commands.DeleteNotification;
using Notifications.Domain.Abstractions;
using Notifications.Domain.ValueObjects;
using FluentAssertions;
using CSharpFunctionalExtensions;

namespace Notifications.Tests.Commands
{
    public class DeleteNotificationCommandHandlerTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly DeleteNotificationCommandHandler _handler;

        public DeleteNotificationCommandHandlerTests()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _handler = new DeleteNotificationCommandHandler(_notificationRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteNotification_WhenCommandIsValid()
        {
            // Arrange
            var command = new DeleteNotificationCommand
            {
                NotificationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _notificationRepositoryMock
                .Setup(x => x.RemoveNotificationAsync(It.IsAny<NotificationId>(), It.IsAny<UserId>()))
                .Returns(Task.FromResult(Result.Success()));

            _notificationRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
            _notificationRepositoryMock.Verify(x => x.RemoveNotificationAsync(It.IsAny<NotificationId>(), It.IsAny<UserId>()), Times.Once);
            _notificationRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNotificationIdIsInvalid()
        {
            // Arrange
            var command = new DeleteNotificationCommand
            {
                NotificationId = Guid.Empty,
                UserId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Invalid notification ID.");
            _notificationRepositoryMock.Verify(x => x.RemoveNotificationAsync(It.IsAny<NotificationId>(), It.IsAny<UserId>()), Times.Never);
            _notificationRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}