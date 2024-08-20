using Moq;
using AutoMapper;
using CSharpFunctionalExtensions;
using Posts.Application.Queries.GetPostLikes;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Application.Tests.Queries
{
    public class GetPostLikesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly GetPostLikesQueryHandler _handler;

        public GetPostLikesQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _handler = new GetPostLikesQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnLikes_WhenLikesExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = UserId.Create(Guid.NewGuid()).Value;

            var likes = new List<Like>
            {
                Like.Create(LikeId.Create(Guid.NewGuid()).Value, userId, PostId.Create(postId).Value).Value
            };

            var users = new List<UserViewModel>
            {
                new UserViewModel { Id = userId }
            };

            _unitOfWorkMock.Setup(u => u.Likes.GetLikesByPostIdAsync(It.IsAny<PostId>()))
                .ReturnsAsync(likes);

            _userServiceMock.Setup(u => u.GetUserListByIdAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(Result.Success(users.AsEnumerable()));

            var likeViewModels = new List<LikeViewModel>
            {
                new LikeViewModel
                {
                    User = new UserViewModel
                    {
                        Id = userId
                    },
                    PostId = postId
                }
            };

            _mapperMock.Setup(m => m.Map<List<LikeViewModel>>(likes)).Returns(likeViewModels);

            // Act
            var result = await _handler.Handle(new GetPostLikesQuery { PostId = postId }, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenPostIdIsInvalid()
        {
            // Arrange
            var invalidPostId = Guid.Empty;

            // Act
            var result = await _handler.Handle(new GetPostLikesQuery { PostId = invalidPostId }, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid PostId format.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionThrown()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.Likes.GetLikesByPostIdAsync(It.IsAny<PostId>()))
                .Returns(Task.FromResult(Result.Failure<List<Like>>("Database error")));

            // Act
            var result = await _handler.Handle(new GetPostLikesQuery { PostId = postId }, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Database error", result.Error);
        }
    }
}
