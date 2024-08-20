using CSharpFunctionalExtensions;
using Moq;
using Posts.Application.Queries.IsPostLikedByUser;
using Posts.Domain.Abstractions;
using Posts.Domain.ValueObjects;

public class IsPostLikedByUserQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IsPostLikedByUserQueryHandler _handler;

    public IsPostLikedByUserQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new IsPostLikedByUserQueryHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new IsPostLikedByUserQuery { PostId = postId, UserId = userId };

        _mockUnitOfWork.Setup(uow => uow.Likes.IsPostLikedByUserAsync(It.IsAny<PostId>(), It.IsAny<UserId>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_InvalidPostId_ReturnsFailureResult()
    {
        // Arrange
        var query = new IsPostLikedByUserQuery { PostId = Guid.Empty, UserId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Invalid PostId format", result.Error);
    }

    [Fact]
    public async Task Handle_InvalidUserId_ReturnsFailureResult()
    {
        // Arrange
        var query = new IsPostLikedByUserQuery { PostId = Guid.NewGuid(), UserId = Guid.Empty };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Invalid UserId format", result.Error);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new IsPostLikedByUserQuery { PostId = postId, UserId = userId };

        _mockUnitOfWork.Setup(uow => uow.Likes.IsPostLikedByUserAsync(It.IsAny<PostId>(), It.IsAny<UserId>()))
            .Returns(Task.FromResult(Result.Failure<bool>("Error while checking if post is liked")));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Error while checking if post is liked", result.Error);
    }
}