using CSharpFunctionalExtensions;
using Moq;
using Posts.Application.Commands.UnlikePost;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Tests.Application.Commands
{
    public class UnlikePostCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UnlikePostCommandHandler _handler;

        public UnlikePostCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new UnlikePostCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UnlikePostCommand
            {
                UserId = Guid.NewGuid(),
                PostId = Guid.NewGuid()
            };

            var like = Like.Create(
                LikeId.Create(Guid.NewGuid()).Value,
                UserId.Create(command.UserId).Value,
                PostId.Create(command.PostId).Value
            ).Value;

            _mockUnitOfWork.Setup(uow => uow.Likes.GetLikeAsync(It.IsAny<PostId>(), It.IsAny<UserId>()))
                .ReturnsAsync(like);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.Likes.RemoveLike(It.IsAny<Like>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidUserId_ReturnsFailureResult()
        {
            // Arrange
            var command = new UnlikePostCommand
            {
                UserId = Guid.Empty,
                PostId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task Handle_LikeNotFound_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UnlikePostCommand
            {
                UserId = Guid.NewGuid(),
                PostId = Guid.NewGuid()
            };

            _mockUnitOfWork.Setup(uow => uow.Likes.GetLikeAsync(It.IsAny<PostId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success<Like>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.Likes.RemoveLike(It.IsAny<Like>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }


        [Fact]
        public async Task Handle_ExceptionThrown_ReturnsFailureResult()
        {
            // Arrange
            var command = new UnlikePostCommand
            {
                UserId = Guid.NewGuid(),
                PostId = Guid.NewGuid()
            };

            _mockUnitOfWork.Setup(uow => uow.Likes.GetLikeAsync(It.IsAny<PostId>(), It.IsAny<UserId>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Error while removing of like", result.Error);
            Assert.Contains("Test exception", result.Error);
        }

        [Fact]
        public async Task Handle_InvalidPostId_ReturnsFailureResult()
        {
            // Arrange
            var command = new UnlikePostCommand
            {
                UserId = Guid.NewGuid(),
                PostId = Guid.Empty
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Invalid PostId format", result.Error);
        }

        [Fact]
        public async Task Handle_ValidRequestButNoLike_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UnlikePostCommand
            {
                UserId = Guid.NewGuid(),
                PostId = Guid.NewGuid()
            };

            _mockUnitOfWork.Setup(uow => uow.Likes.GetLikeAsync(It.IsAny<PostId>(), It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success<Like>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.Likes.RemoveLike(It.IsAny<Like>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

    }
}