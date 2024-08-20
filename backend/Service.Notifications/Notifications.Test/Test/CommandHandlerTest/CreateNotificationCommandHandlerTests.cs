using Moq;
using Notifications.Domain.Abstractions;
using Notifications.Application.Commands.CreateNotification;
using Notifications.Domain.ValueObjects;
using Notifications.Domain.Entities;
using FluentAssertions;
using CSharpFunctionalExtensions;

namespace Notifications.Tests.Commands
{
    public class CreateNotificationCommandHandlerTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly CreateNotificationCommandHandler _handler;

        public CreateNotificationCommandHandlerTests()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _handler = new CreateNotificationCommandHandler(_notificationRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateNotification_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreateNotificationCommand
            {
                UserId = Guid.NewGuid(),
                PostId = Guid.NewGuid(),
                Content = "New Notification",
                Type = NotificationType.Like
            };

            _notificationRepositoryMock
                .Setup(x => x.AddNotificationAsync(It.IsAny<Notification>()))
                .Returns(Task.FromResult(Result.Success()));

            _notificationRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            _notificationRepositoryMock.Verify(x => x.AddNotificationAsync(It.IsAny<Notification>()), Times.Once);
            _notificationRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            // Arrange
            var command = new CreateNotificationCommand
            {
                UserId = Guid.Empty,
                PostId = Guid.NewGuid(),
                Content = "New Notification",
                Type = NotificationType.Like
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Invalid UserId format.");
            _notificationRepositoryMock.Verify(x => x.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
            _notificationRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}