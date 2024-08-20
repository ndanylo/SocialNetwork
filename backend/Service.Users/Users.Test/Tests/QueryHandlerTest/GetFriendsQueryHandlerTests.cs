using Moq;
using Users.Application.Queries.GetFriends;
using Users.Application.ViewModels;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using Users.Domain.Entities;
using AutoMapper;
using OnlineChat.Application.Users.Queries.GetFriends;
using CSharpFunctionalExtensions;

namespace Users.Tests.Application.Queries
{
    public class GetFriendsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetFriendsQueryHandler _handler;

        public GetFriendsQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetFriendsQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            // Arrange
            var query = new GetFriendsQuery
            {
                UserId = Guid.Empty
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenValidUserIdProvided()
        {
            // Arrange
            var query = new GetFriendsQuery
            {
                UserId = Guid.NewGuid()
            };

            var friends = new List<User>
            {
                User.Default,
                User.Default
            };

            var friendsViewModels = new List<UserViewModel>
            {
                new UserViewModel(),
                new UserViewModel()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserFriendsAsync(It.IsAny<UserId>()))
                .ReturnsAsync(friends);
                
            _mapperMock.Setup(m => m.Map<List<UserViewModel>>(friends))
                .Returns(friendsViewModels);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(friendsViewModels.Count, result.Value.Count);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRetrievingFriendsThrowsException()
        {
            // Arrange
            var query = new GetFriendsQuery
            {
                UserId = Guid.NewGuid()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserFriendsAsync(It.IsAny<UserId>()))
                .ReturnsAsync(Result.Failure<List<User>>("Database error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Database error", result.Error);
        }
    }
}
