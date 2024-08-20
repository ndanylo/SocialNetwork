using Moq;
using Notifications.Application.Commands.DeleteAllNotifications;
using Notifications.Domain.Abstractions;
using Notifications.Domain.ValueObjects;
using FluentAssertions;
using CSharpFunctionalExtensions;

namespace Notifications.Tests.Commands
{
    public class DeleteAllNotificationsCommandHandlerTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly DeleteAllNotificationsCommandHandler _handler;

        public DeleteAllNotificationsCommandHandlerTests()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _handler = new DeleteAllNotificationsCommandHandler(_notificationRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAllNotifications_WhenUserIdIsValid()
        {
            // Arrange
            var command = new DeleteAllNotificationsCommand
            {
                UserId = Guid.NewGuid()
            };

            _notificationRepositoryMock
                .Setup(x => x.RemoveAllNotificationsAsync(It.IsAny<UserId>()))
                .Returns(Task.FromResult(Result.Success()));

            _notificationRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _notificationRepositoryMock.Verify(x => x.RemoveAllNotificationsAsync(It.IsAny<UserId>()), Times.Once);
            _notificationRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            // Arrange
            var command = new DeleteAllNotificationsCommand
            {
                UserId = Guid.Empty
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Invalid user ID.");
            _notificationRepositoryMock.Verify(x => x.RemoveAllNotificationsAsync(It.IsAny<UserId>()), Times.Never);
            _notificationRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}