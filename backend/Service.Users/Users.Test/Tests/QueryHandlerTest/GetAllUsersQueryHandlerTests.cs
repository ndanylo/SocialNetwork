using Moq;
using Users.Application.Queries.GetAllUsers;
using Users.Application.ViewModels;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using AutoMapper;
using OnlineChat.Application.Users.Queries.GetAllUsers;
using Users.Domain.Entities;

namespace Users.Tests.Application.Queries
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllUsersQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            // Arrange
            var query = new GetAllUsersQuery
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
            var query = new GetAllUsersQuery
            {
                UserId = Guid.NewGuid()
            };

            var users = new List<User>
            {
                User.Default,
                User.Default
            };

            var userViewModels = new List<UserViewModel>
            {
                new UserViewModel(),
                new UserViewModel()
            };

            _unitOfWorkMock.Setup(u => u.Users.GetAllUsersAsync(It.IsAny<UserId>()))
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
        public async Task Handle_ShouldReturnEmptyList_WhenNoUsersFound()
        {
            // Arrange
            var query = new GetAllUsersQuery
            {
                UserId = Guid.NewGuid()
            };

            var users = new List<User>();
            var userViewModels = new List<UserViewModel>();

            _unitOfWorkMock.Setup(u => u.Users.GetAllUsersAsync(It.IsAny<UserId>()))
                           .ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<UserViewModel>>(users))
                       .Returns(userViewModels);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenMappingFails()
        {
            // Arrange
            var query = new GetAllUsersQuery
            {
                UserId = Guid.NewGuid()
            };

            var users = new List<User>();

            _unitOfWorkMock.Setup(u => u.Users.GetAllUsersAsync(It.IsAny<UserId>()))
                           .ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<UserViewModel>>(users))
                       .Throws(new Exception("Mapping error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("An error occurred while mapping users.", result.Error);
        }
    }
}
