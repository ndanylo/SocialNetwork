using AutoMapper;
using Chats.Application.Queries.GetChatByUser;
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
    public class GetChatByUserQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<GetChatByUserQueryHandler>> _mockLogger;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly GetChatByUserQueryHandler _handler;

        public GetChatByUserQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<GetChatByUserQueryHandler>>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();

            _handler = new GetChatByUserQueryHandler(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockNotificationService.Object,
                _mockMapper.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var query = new GetChatByUserQuery
            {
                UserSenderId = Guid.NewGuid(),
                UserReceiverId = Guid.NewGuid()
            };

            var chatId = ChatId.Create(Guid.NewGuid()).Value;

            var chatUser1 = ChatUser.Create(UserId.Create(query.UserSenderId).Value, chatId).Value;
            var chatUser2 = ChatUser.Create(UserId.Create(Guid.NewGuid()).Value, chatId).Value;
            var chatUser3 = ChatUser.Create(UserId.Create(Guid.NewGuid()).Value, chatId).Value;

            var chatUsers = new List<ChatUser> { chatUser1, chatUser2, chatUser3 };

            var chatRoom = Chat.Create(chatId, ChatRoomName.Create("Test Chat").Value, false, chatUsers).Value;

            _mockUnitOfWork.Setup(uow => uow.Chats.GetPrivateChatByUsersAsync(It.IsAny<UserId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success(chatRoom));

            _mockUnitOfWork.Setup(uow => uow.Messages.GetUnreadMessageCountForUserAsync(It.IsAny<UserId>(), It.IsAny<ChatId>()))
                .ReturnsAsync(Result.Success(0));

            _mockUserService.Setup(us => us.GetUserListByIdAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult(Result.Success(new List<UserViewModel>
                {
                    new UserViewModel { Id = query.UserSenderId },
                    new UserViewModel { Id = query.UserReceiverId }
                }.AsEnumerable())));

            _mockMapper.Setup(m => m.Map<ChatViewModel>(
                It.IsAny<Chat>(),
                It.IsAny<Action<IMappingOperationOptions<object, ChatViewModel>>>()))
                .Returns(new ChatViewModel());


            _mockNotificationService.Setup(ns => ns.ReadChatAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}