using Moq;
using Users.Application.Commands.SendFriendRequest;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using AutoMapper;
using Posts.Application.Services.Abstractions;

namespace Users.Tests.Application.Commands
{
    public class SendFriendRequestCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SendFriendRequestCommandHandler _handler;

        public SendFriendRequestCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _notificationServiceMock = new Mock<INotificationService>();
            _mapperMock = new Mock<IMapper>();
            _handler = new SendFriendRequestCommandHandler(_notificationServiceMock.Object, _unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSenderIdIsInvalid()
        {
            // Arrange
            var command = new SendFriendRequestCommand
            {
                SenderId = Guid.Empty,
                ReceiverId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid sender ID.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenReceiverIdIsInvalid()
        {
            // Arrange
            var command = new SendFriendRequestCommand
            {
                SenderId = Guid.NewGuid(),
                ReceiverId = Guid.Empty
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid receiver ID.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSenderOrReceiverNotFound()
        {
            // Arrange
            var command = new SendFriendRequestCommand
            {
                SenderId = Guid.NewGuid(),
                ReceiverId = Guid.NewGuid()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(It.IsAny<UserId>()))
                           .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Sender or receiver not found.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUsersAreAlreadyFriends()
        {
            // Arrange
            var command = new SendFriendRequestCommand
            {
                SenderId = Guid.NewGuid(),
                ReceiverId = Guid.NewGuid()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(It.IsAny<UserId>()))
                           .ReturnsAsync(User.Default);
            _unitOfWorkMock.Setup(u => u.Users.AreFriendsAsync(It.IsAny<UserId>(), It.IsAny<UserId>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Users already have friendship.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenFriendRequestAlreadyExists()
        {
            // Arrange
            var command = new SendFriendRequestCommand
            {
                SenderId = Guid.NewGuid(),
                ReceiverId = Guid.NewGuid()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(It.IsAny<UserId>()))
                           .ReturnsAsync(User.Default);
            _unitOfWorkMock.Setup(u => u.Users.AreFriendsAsync(It.IsAny<UserId>(), It.IsAny<UserId>()))
                           .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.FriendRequests.ExistsAsync(It.IsAny<UserId>(), It.IsAny<UserId>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Friend request already sent.", result.Error);
        }
    }
}
