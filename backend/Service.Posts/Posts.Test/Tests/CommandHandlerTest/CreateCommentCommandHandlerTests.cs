using AutoMapper;
using CSharpFunctionalExtensions;
using Moq;
using Posts.Application.Commands.CreateComment;
using Posts.Application.Services.Abstractions;
using Posts.Application.Services.Contracts;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Tests.Application.Commands
{
    public class CreateCommentCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly CreateCommentCommandHandler _handler;

        public CreateCommentCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockUserService = new Mock<IUserService>();

            _handler = new CreateCommentCommandHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockNotificationService.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreateCommentCommand
            {
                PostId = Guid.NewGuid(),
                Content = "Test comment",
                UserId = Guid.NewGuid()
            };

            var postId = PostId.Create(command.PostId).Value;
            var userId = UserId.Create(command.UserId).Value;
            var post = Post.Create(postId, userId, PostContent.Create("Test post").Value, PhotoUrl.Create("image").Value).Value;

            _mockUnitOfWork.Setup(uow => uow.Posts.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            _mockUserService.Setup(us => us.GetUserByIdAsync(userId))
                .ReturnsAsync(Result.Success(new UserViewModel { Id = command.UserId }));

            _mockUnitOfWork.Setup(uow => uow.Comments)
                .Returns(new Mock<ICommentRepository>().Object);

            _mockMapper.Setup(m => m.Map<CommentViewModel>(It.IsAny<Comment>()))
                .Returns(new CommentViewModel());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.Comments.AddCommentAsync(It.IsAny<Comment>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
            _mockNotificationService.Verify(ns => ns.CreateNotificationAsync(
                It.IsAny<UserId>(),
                It.IsAny<PostId>(),
                It.IsAny<string>(),
                NotificationType.Comment
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidUserId_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateCommentCommand
            {
                PostId = Guid.NewGuid(),
                Content = "Test comment",
                UserId = Guid.Empty
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateCommentCommand
            {
                PostId = Guid.NewGuid(),
                Content = "Test comment",
                UserId = Guid.NewGuid()
            };

            var userId = UserId.Create(command.UserId).Value;

            _mockUserService.Setup(us => us.GetUserByIdAsync(userId))
                .ReturnsAsync(Result.Failure<UserViewModel>("User not found."));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("User not found.", result.Error);
        }

        [Fact]
        public async Task Handle_PostNotFound_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateCommentCommand
            {
                PostId = Guid.NewGuid(),
                Content = "Test comment",
                UserId = Guid.NewGuid()
            };

            var postId = PostId.Create(command.PostId).Value;
            var userId = UserId.Create(command.UserId).Value;

            _mockUserService.Setup(us => us.GetUserByIdAsync(userId))
                .ReturnsAsync(Result.Success(new UserViewModel { Id = command.UserId }));
            
             _mockUnitOfWork.Setup(uow => uow.Comments.AddCommentAsync(It.IsAny<Comment>()))
                .ReturnsAsync(Result.Failure<Comment>("Post was not added"));

            _mockUnitOfWork.Setup(uow => uow.Posts.GetPostByIdAsync(postId))
                .ReturnsAsync(Post.Default);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Post was not added", result.Error);
        }
    }
}