using Moq;
using OnlineChat.Application.Comments.Commands.DeleteComment;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Tests.Application.Commands
{
    public class DeleteCommentCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteCommentCommandHandler _handler;

        public DeleteCommentCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new DeleteCommentCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var command = new DeleteCommentCommand
            {
                CommentId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var comment = Comment.Create(
                CommentId.Create(command.CommentId).Value,
                UserId.Create(command.UserId).Value,
                PostId.Create(Guid.NewGuid()).Value,
                CommentContent.Create("Test comment").Value
            ).Value;

            _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(It.IsAny<CommentId>()))
                .ReturnsAsync(comment);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.Comments.RemoveComment(It.IsAny<Comment>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_CommentNotFound_ReturnsFailureResult()
        {
            // Arrange
            var command = new DeleteCommentCommand
            {
                CommentId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(It.IsAny<CommentId>()))
                .ReturnsAsync(Comment.Default);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Comment not found or user is not authorized.", result.Error);
        }

        [Fact]
        public async Task Handle_UserNotAuthorized_ReturnsFailureResult()
        {
            // Arrange
            var command = new DeleteCommentCommand
            {
                CommentId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var comment = Comment.Create(
                CommentId.Create(command.CommentId).Value,
                UserId.Create(Guid.NewGuid()).Value,
                PostId.Create(Guid.NewGuid()).Value,
                CommentContent.Create("Test comment").Value
            ).Value;

            _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentByIdAsync(It.IsAny<CommentId>()))
                .ReturnsAsync(comment);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Comment not found or user is not authorized.", result.Error);
        }
    }
}