using Moq;
using Posts.Application.Commands.LikePost;
using Posts.Application.Services.Abstractions;
using Posts.Application.Services.Contracts;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;
namespace Posts.Tests.Application.Commands
{
    public class LikePostCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly LikePostCommandHandler _handler;

        public LikePostCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockNotificationService = new Mock<INotificationService>();
            _handler = new LikePostCommandHandler(_mockUnitOfWork.Object, _mockNotificationService.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var command = new LikePostCommand
            {
                UserId = Guid.NewGuid(),
                PostId = Guid.NewGuid()
            };

            var post = Post.Create(
                PostId.Create(command.PostId).Value,
                UserId.Create(Guid.NewGuid()).Value,
                PostContent.Create("Test post").Value,
                PhotoUrl.Create("avatar").Value
            ).Value;

            _mockUnitOfWork.Setup(uow => uow.Posts.GetPostByIdAsync(It.IsAny<PostId>()))
                .ReturnsAsync(post);

            _mockUnitOfWork.Setup(uow => uow.Likes)
                .Returns(new Mock<ILikeRepository>().Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.Likes.AddLikeAsync(It.IsAny<Like>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
            _mockNotificationService.Verify(ns => ns.CreateNotificationAsync(
                It.IsAny<UserId>(),
                It.IsAny<PostId>(),
                It.IsAny<string>(),
                It.IsAny<NotificationType>()
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidUserId_ReturnsFailureResult()
        {
            // Arrange
            var command = new LikePostCommand
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
    }
}