using Moq;
using AutoMapper;
using CSharpFunctionalExtensions;
using OnlineChat.Application.FriendRequests.Queries.GetSentFriendRequests;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using Users.Application.ViewModels;

namespace OnlineChat.Tests.Application.FriendRequests.Queries
{
    public class GetSentFriendRequestsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetSentFriendRequestsQueryHandler _handler;

        public GetSentFriendRequestsQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetSentFriendRequestsQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            // Arrange
            var query = new GetSentFriendRequestsQuery
            {
                UserId = Guid.Empty // Invalid Guid
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var validUserId = UserId.Create(Guid.NewGuid()).Value;
            var query = new GetSentFriendRequestsQuery
            {
                UserId = validUserId
            };

            _unitOfWorkMock
                .Setup(repo => repo.FriendRequests.GetSentFriendRequestsAsync(It.IsAny<UserId>()))
                .ReturnsAsync(Result.Failure<List<FriendRequest>>("Database error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Error while receiving sent friend requests", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedUserViewModels_WhenFriendRequestsAreFound()
        {
            // Arrange
            var validUserId = UserId.Create(Guid.NewGuid()).Value;
            var query = new GetSentFriendRequestsQuery
            {
                UserId = validUserId
            };

            var friendRequests = new List<FriendRequest>
            {
                FriendRequest.Create(FriendRequestId.Create(Guid.NewGuid()).Value,
                                    validUserId,
                                    UserId.Create(Guid.NewGuid()).Value,
                                    User.Default,
                                    User.Default).Value,
                FriendRequest.Create(FriendRequestId.Create(Guid.NewGuid()).Value,
                                    validUserId,
                                    UserId.Create(Guid.NewGuid()).Value,
                                    User.Default,
                                    User.Default).Value
            };

            var userViewModels = new List<UserViewModel>
            {
                new UserViewModel { UserName = "User1" },
                new UserViewModel { UserName = "User2" }
            };

            _unitOfWorkMock
                .Setup(repo => repo.FriendRequests.GetSentFriendRequestsAsync(It.IsAny<UserId>()))
                .ReturnsAsync(friendRequests);

            _mapperMock
                .Setup(mapper => mapper.Map<List<UserViewModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns(userViewModels);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userViewModels.Count, result.Value.Count);
            Assert.Equal("User1", result.Value[0].UserName);
            Assert.Equal("User2", result.Value[1].UserName);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoFriendRequestsAreFound()
        {
            // Arrange
            var validUserId = UserId.Create(Guid.NewGuid()).Value;
            var query = new GetSentFriendRequestsQuery
            {
                UserId = validUserId
            };

            _unitOfWorkMock
                .Setup(repo => repo.FriendRequests.GetSentFriendRequestsAsync(It.IsAny<UserId>()))
                .ReturnsAsync(new List<FriendRequest>());

            _mapperMock
                .Setup(mapper => mapper.Map<List<UserViewModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns(new List<UserViewModel>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }
    }
}
