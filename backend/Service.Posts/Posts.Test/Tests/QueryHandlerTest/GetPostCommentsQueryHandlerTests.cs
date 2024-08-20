using AutoMapper;
using CSharpFunctionalExtensions;
using Moq;
using Posts.Application.Queries.GetPostComments;
using Posts.Application.Services.Abstractions;
using Posts.Application.ViewModels;
using Posts.Domain.Abstractions;
using Posts.Domain.Entities;
using Posts.Domain.ValueObjects;

namespace Posts.Tests.Application.Queries
{
    public class GetPostCommentsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly GetPostCommentsQueryHandler _handler;

        public GetPostCommentsQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _handler = new GetPostCommentsQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var query = new GetCommentsQuery { PostId = postId };
            var comments = new List<Comment>
            {
                Comment.Create(CommentId.Create(Guid.NewGuid()).Value, UserId.Create(Guid.NewGuid()).Value, PostId.Create(postId).Value, CommentContent.Create("Test comment").Value).Value
            };

            _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentsByPostId(It.IsAny<PostId>()))
                .ReturnsAsync(comments);

            _mockUserService.Setup(us => us.GetUserListByIdAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(Result.Success<IEnumerable<UserViewModel>>(new List<UserViewModel> { new UserViewModel { Id = comments[0].UserId } }));

            _mockMapper.Setup(m => m.Map<CommentViewModel>(It.IsAny<Comment>()))
                .Returns(new CommentViewModel());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task Handle_InvalidPostId_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetCommentsQuery { PostId = Guid.Empty };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Invalid PostId format", result.Error);
        }

        [Fact]
        public async Task Handle_NoComments_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetCommentsQuery { PostId = Guid.NewGuid() };

            _mockUnitOfWork.Setup(uow => uow.Comments.GetCommentsByPostId(It.IsAny<PostId>()))
                .ReturnsAsync(new List<Comment>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }
    }
}
