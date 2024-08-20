using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Posts.Application.Commands.CreatePost;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Tests.Application.Commands
{
    public class CreatePostCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<CreatePostCommandHandler>> _mockLogger;
        private readonly CreatePostCommandHandler _handler;

        public CreatePostCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockImageService = new Mock<IImageService>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<CreatePostCommandHandler>>();

            _handler = new CreatePostCommandHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockImageService.Object,
                _mockLogger.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreatePostCommand
            {
                Content = "Test content",
                UserId = Guid.NewGuid(),
                Image = Mock.Of<IFormFile>()
            };

            var user = new UserViewModel { Id = command.UserId };
            _mockUserService.Setup(us => us.GetUserByIdAsync(It.IsAny<UserId>()))
                .ReturnsAsync(Result.Success(user));

            _mockImageService.Setup(s => s.SaveImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(PhotoUrl.Create("avatar").Value);

            _mockMapper.Setup(m => m.Map<Post, PostViewModel>(It.IsAny<Post>(), It.IsAny<Action<IMappingOperationOptions<Post, PostViewModel>>>()))
                .Returns(new PostViewModel());

            _mockUnitOfWork.Setup(uow => uow.Posts)
                .Returns(new Mock<IPostRepository>().Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.Posts.AddPostAsync(It.IsAny<Post>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidUserId_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreatePostCommand
            {
                Content = "Test content",
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
            var command = new CreatePostCommand
            {
                Content = "Test content",
                UserId = Guid.NewGuid()
            };

            _mockUserService.Setup(us => us.GetUserByIdAsync(It.IsAny<UserId>()))
                .ReturnsAsync(Result.Failure<UserViewModel>("User not found"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Error fetching user data", result.Error);
        }
    }
}