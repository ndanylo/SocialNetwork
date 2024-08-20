using Moq;
using AutoMapper;
using Users.Application.Queries.GetReceivedFriendRequests;
using Users.Application.ViewModels;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Tests.Application.Queries
{
    public class GetReceivedFriendRequestsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetReceivedFriendRequestsQueryHandler _handler;

        public GetReceivedFriendRequestsQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetReceivedFriendRequestsQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            // Arrange
            var query = new GetReceivedFriendRequestsQuery
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
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var query = new GetReceivedFriendRequestsQuery
            {
                UserId = Guid.NewGuid()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(It.IsAny<UserId>()))
                           .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Error while receiving friend requests", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenReceivedFriendRequestsAreRetrieved()
        {
            // Arrange
            var query = new GetReceivedFriendRequestsQuery
            {
                UserId = Guid.NewGuid()
            };

            var currentUser = User.Default;
            var senderId1 = UserId.Create(Guid.NewGuid()).Value;
            var senderId2 = UserId.Create(Guid.NewGuid()).Value;
            var userId = UserId.Create(query.UserId).Value;

            var sender1 = User.Default;
            var sender2 = User.Default;

            var friendRequest1 = FriendRequest.Create(FriendRequestId.Create(Guid.NewGuid()).Value,
                                                    senderId1,
                                                    userId,
                                                    sender1,
                                                    currentUser).Value;
            var friendRequest2 = FriendRequest.Create(FriendRequestId.Create(Guid.NewGuid()).Value,
                                                    senderId2,
                                                    userId,
                                                    sender2,
                                                    currentUser).Value;

            var friendRequests = new List<FriendRequest> { friendRequest1, friendRequest2 };

            var users = new List<User> { sender1, sender2 };

            var userViewModels = new List<UserViewModel>
            {
                new UserViewModel(),
                new UserViewModel()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(It.IsAny<UserId>()))
                           .ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.FriendRequests.GetReceivedFriendRequestsAsync(It.IsAny<UserId>()))
                           .ReturnsAsync(friendRequests);
            _unitOfWorkMock.Setup(u => u.Users.GetUsersByIds(It.IsAny<List<UserId>>()))
                           .ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<UserViewModel>>(users))
                       .Returns(userViewModels);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userViewModels.Count, result.Value.Count);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var query = new GetReceivedFriendRequestsQuery
            {
                UserId = Guid.NewGuid()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(It.IsAny<UserId>()))
                           .ReturnsAsync(User.Default);
            _unitOfWorkMock.Setup(u => u.FriendRequests.GetReceivedFriendRequestsAsync(It.IsAny<UserId>()))
                           .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Error while receiving friend requests: Database error", result.Error);
        }
    }
}
