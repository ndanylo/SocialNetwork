using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Chats.Domain.Abstractions;
using Chats.Application.Commands.CreateGroupChat;
using AutoMapper;
using Chats.Application.Services.Abstractions;
using CSharpFunctionalExtensions;
using Chats.Application.ViewModels;
using Chats.Domain.ValueObjects;
using Chats.Domain.Entities;

namespace Chats.Application.Tests.Commands
{
    public class CreateGroupChatCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateGroupChatCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly CreateGroupChatCommandHandler _handler;

        public CreateGroupChatCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateGroupChatCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _handler = new CreateGroupChatCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenChatNameIsEmpty()
        {
            // Arrange
            var request = new CreateGroupChatCommand
            {
                Name = string.Empty,
                UserIds = new List<Guid> { Guid.NewGuid() },
                CreatorId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Chat name is required.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdsAreEmpty()
        {
            // Arrange
            var request = new CreateGroupChatCommand
            {
                Name = "Test Chat",
                UserIds = new List<Guid>(),
                CreatorId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("At least one user ID is required.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDetailsNotFound()
        {
            // Arrange
            var request = new CreateGroupChatCommand
            {
                Name = "Test Chat",
                UserIds = new List<Guid> { Guid.NewGuid() },
                CreatorId = Guid.NewGuid()
            };

            _userServiceMock
                .Setup(us => us.GetUserListByIdAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(Result.Failure<IEnumerable<UserViewModel>>("User details not found"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("User details not found");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenGroupChatIsCreated()
        {
            // Arrange
            var request = new CreateGroupChatCommand
            {
                Name = "Test Group Chat",
                UserIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                CreatorId = Guid.NewGuid()
            };

            var userViewModels = new List<UserViewModel>
            {
                new UserViewModel { Id = request.UserIds[0] },
                new UserViewModel { Id = request.UserIds[1] },
                new UserViewModel { Id = request.CreatorId }
            };
            var chatId = ChatId.Create(Guid.NewGuid()).Value;

            var chatUser1 = ChatUser.Create(UserId.Create(request.UserIds[0]).Value, chatId).Value;
            var chatUser2 = ChatUser.Create(UserId.Create(request.UserIds[1]).Value, chatId).Value;
            var chatUser3 = ChatUser.Create(UserId.Create(request.CreatorId).Value, chatId).Value;

            var chatUsers = new List<ChatUser> { chatUser1, chatUser2, chatUser3 };

            _userServiceMock
                .Setup(us => us.GetUserListByIdAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(Result.Success<IEnumerable<UserViewModel>>(userViewModels));

            _unitOfWorkMock
                .Setup(uow => uow.Chats.CreateGroupChat(It.IsAny<ChatRoomName>(), It.IsAny<List<UserId>>()))
                .Returns(Result.Success(Chat.Create(ChatId.Create(Guid.NewGuid()).Value,
                                        ChatRoomName.Create("Test Group Chat").Value, 
                                        true, 
                                        chatUsers).Value));

            _mapperMock
                .Setup(m => m.Map<ChatViewModel>(It.IsAny<Chat>()))
                .Returns(new ChatViewModel());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
