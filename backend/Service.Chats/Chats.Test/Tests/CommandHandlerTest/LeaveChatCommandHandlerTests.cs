using Chats.Application.Commands.LeaveChat;
using Chats.Domain.Abstractions;
using Chats.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using Moq;

public class LeaveChatCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly LeaveChatCommandHandler _handler;

    public LeaveChatCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new LeaveChatCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserSuccessfullyLeavesChat()
    {
        // Arrange
        var command = new LeaveChatCommand
        {
            UserId = Guid.NewGuid(),
            ChatRoomId = Guid.NewGuid()
        };

        _unitOfWorkMock
            .Setup(uow => uow.Chats.LeaveChatAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()))
            .ReturnsAsync(Result.Success(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _unitOfWorkMock.Verify(uow => uow.Chats.LeaveChatAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenInvalidUserId()
    {
        // Arrange
        var command = new LeaveChatCommand
        {
            UserId = Guid.Empty, // Invalid UserId
            ChatRoomId = Guid.NewGuid()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid user ID.", result.Error);
        _unitOfWorkMock.Verify(uow => uow.Chats.LeaveChatAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenInvalidChatRoomId()
    {
        // Arrange
        var command = new LeaveChatCommand
        {
            UserId = Guid.NewGuid(),
            ChatRoomId = Guid.Empty // Invalid ChatRoomId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid chat room ID.", result.Error);
        _unitOfWorkMock.Verify(uow => uow.Chats.LeaveChatAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLeaveChatFails()
    {
        // Arrange
        var command = new LeaveChatCommand
        {
            UserId = Guid.NewGuid(),
            ChatRoomId = Guid.NewGuid()
        };

        _unitOfWorkMock
            .Setup(uow => uow.Chats.LeaveChatAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()))
            .ReturnsAsync(Result.Failure<bool>("Leave chat failed."));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Leave chat failed.", result.Error);
        _unitOfWorkMock.Verify(uow => uow.Chats.LeaveChatAsync(It.IsAny<ChatId>(), It.IsAny<UserId>()), Times.Once);
    }
}
