using Moq;
using Microsoft.Extensions.Logging;
using Chats.Domain.Abstractions;
using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using Chats.Application.Services.Abstractions;
using Chats.Application.Commands.ReadChatMessages;
using CSharpFunctionalExtensions;

namespace Chats.Application.Tests.Commands
{
    public class ReadChatMessagesCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<ReadChatMessagesCommandHandler>> _mockLogger;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly ReadChatMessagesCommandHandler _handler;

        public ReadChatMessagesCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<ReadChatMessagesCommandHandler>>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockUserService = new Mock<IUserService>();

            _handler = new ReadChatMessagesCommandHandler(
                _mockNotificationService.Object,
                _mockUnitOfWork.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_SuccessfullyReadsMessages()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var chatRoomId = Guid.NewGuid();
            var command = new ReadChatCommand { UserId = userId, ChatRoomId = chatRoomId };

            var userIdResult = UserId.Create(userId).Value;
            var chatIdResult = ChatId.Create(chatRoomId).Value;

            var messages = new List<Message>
            {
                Message.Create(MessageId.Create(Guid.NewGuid()).Value, ChatUserId.Create(userIdResult, chatIdResult).Value, chatIdResult, Chat.Default, MessageContent.Create("Test message 1").Value).Value,
                Message.Create(MessageId.Create(Guid.NewGuid()).Value, ChatUserId.Create(userIdResult, chatIdResult).Value, chatIdResult, Chat.Default, MessageContent.Create("Test message 2").Value).Value
            };
            var chatUser1 = ChatUser.Create(userIdResult, chatIdResult).Value;
            var chatUser2 = ChatUser.Create(UserId.Create(Guid.NewGuid()).Value, chatIdResult).Value;
            var chatUser3 = ChatUser.Create(UserId.Create(Guid.NewGuid()).Value, chatIdResult).Value;

            var chatUsers = new List<ChatUser> { chatUser1, chatUser2, chatUser3 };

            var chatRoom = Chat.Create(chatIdResult, ChatRoomName.Create("Test Chat").Value, false, chatUsers).Value;
            var otherUserId = Guid.NewGuid();
            var otherUser = ChatUser.Create(UserId.Create(otherUserId).Value, chatIdResult).Value;
            chatRoom.AddUser(otherUser);

            _mockUnitOfWork.Setup(uow => uow.Messages.ReadChatMessagesAsync(It.Is<ChatId>(c => c.Id == chatRoomId), It.Is<UserId>(u => u.Id == userId)))
                .ReturnsAsync(Result.Success(messages));

            _mockUnitOfWork.Setup(uow => uow.Chats.GetChatRoomAsync(It.Is<ChatId>(c => c.Id == chatRoomId), It.Is<UserId>(u => u.Id == userId)))
                .ReturnsAsync(Result.Success(chatRoom));

            _mockUserService.Setup(us => us.GetUserByIdAsync(otherUserId))
                .ReturnsAsync(Result.Success(new ViewModels.UserViewModel { Id = otherUserId }));

            _mockNotificationService.Setup(ns => ns.SetMessageReadAsync(chatRoomId, It.IsAny<Guid>(), It.IsAny<ViewModels.UserViewModel>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Exactly(2));
            _mockUserService.Verify(us => us.GetUserByIdAsync(otherUserId), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidUserId_ReturnsFailure()
        {
            // Arrange
            var command = new ReadChatCommand { UserId = Guid.Empty, ChatRoomId = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task Handle_InvalidChatRoomId_ReturnsFailure()
        {
            // Arrange
            var command = new ReadChatCommand { UserId = Guid.NewGuid(), ChatRoomId = Guid.Empty };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid chat room ID.", result.Error);
        }

        [Fact]
        public async Task Handle_ReadMessagesFailure_ReturnsFailure()
        {
            // Arrange
            var command = new ReadChatCommand { UserId = Guid.NewGuid(), ChatRoomId = Guid.NewGuid() };

            _mockUnitOfWork.Setup(uow => uow.Messages.ReadChatMessagesAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Failure<List<Message>>("Failed to read messages"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Failed to read messages", result.Error);
        }

        [Fact]
        public async Task Handle_GetChatRoomFailure_ReturnsFailure()
        {
            // Arrange
            var command = new ReadChatCommand { UserId = Guid.NewGuid(), ChatRoomId = Guid.NewGuid() };

            _mockUnitOfWork.Setup(uow => uow.Messages.ReadChatMessagesAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success(new List<Message>()));

            _mockUnitOfWork.Setup(uow => uow.Chats.GetChatRoomAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Failure<Chat>("Failed to get chat room"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Failed to get chat room", result.Error);
        }
    }
}