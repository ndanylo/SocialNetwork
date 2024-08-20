using AutoMapper;
using Chats.Application.Commands.SendMessage;
using Chats.Application.Services.Abstractions;
using Chats.Application.ViewModels;
using Chats.Domain.Abstractions;
using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chats.Application.Tests.Commands
{
    public class SendMessageCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<SendMessageCommandHandler>> _mockLogger;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly SendMessageCommandHandler _handler;

        public SendMessageCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<SendMessageCommandHandler>>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();

            _handler = new SendMessageCommandHandler(
                _mockUnitOfWork.Object,
                _mockNotificationService.Object,
                _mockMapper.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var command = new SendMessageCommand
            {
                ChatId = Guid.NewGuid(),
                Content = "Test message",
                SenderId = Guid.NewGuid()
            };

            var chatId = ChatId.Create(command.ChatId).Value;

            var chatUser1 = ChatUser.Create(UserId.Create(command.SenderId).Value, chatId).Value;
            var chatUser2 = ChatUser.Create(UserId.Create(Guid.NewGuid()).Value, chatId).Value;
            var chatUser3 = ChatUser.Create(UserId.Create(Guid.NewGuid()).Value, chatId).Value;

            var chatUsers = new List<ChatUser> { chatUser1, chatUser2, chatUser3 };

            var chatRoom = Chat.Create(chatId, ChatRoomName.Create("Test Chat").Value, false, chatUsers).Value;
            var sender = ChatUser.Create(UserId.Create(command.SenderId).Value, chatRoom.Id).Value;
            chatRoom.AddUser(sender);

            _mockUnitOfWork.Setup(uow => uow.Chats.GetChatRoomAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success(chatRoom));

            _mockUnitOfWork.Setup(uow => uow.Messages.SendMessageAsync(It.IsAny<Message>()))
                .Returns(Task.FromResult(Result.Success(Message.Default)));


            _mockUserService.Setup(us => us.GetUserListByIdAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult(Result.Success(new List<UserViewModel> { new UserViewModel { Id = command.SenderId } }.AsEnumerable())));

            _mockMapper.Setup(m => m.Map<MessageViewModel>(
                It.IsAny<object>(),
                It.IsAny<Action<IMappingOperationOptions<object, MessageViewModel>>>()))
                .Returns(new MessageViewModel());


            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}