using AutoMapper;
using CSharpFunctionalExtensions;
using Moq;
using Users.Application.Queries.GetUsersDetails;
using Users.Application.ViewModels;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Application.Tests.Queries.GetUsersDetails
{
    public class GetUsersDetailsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetUsersDetailsQueryHandler _handler;

        public GetUsersDetailsQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetUsersDetailsQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WithValidData()
        {
            // Arrange
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var userIdObjects = userIds.Select(id => UserId.Create(id).Value).ToList();
            var users = userIdObjects.Select(id =>
                User.Create(id, "email@example.com", "username", "John", "Doe", "City").Value
            ).ToList();

            var userViewModels = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                City = u.City,
                Avatar = new byte[] { 1, 2, 3 }
            }).ToList();

            _unitOfWorkMock.Setup(uow => uow.Users.GetUsersByIds(It.IsAny<List<UserId>>()))
                .ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<UserViewModel>>(users))
                .Returns(userViewModels);

            var request = new GetUsersDetailsQuery { UserIds = userIds };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(userViewModels, result.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInvalidUserId()
        {
            // Arrange
            var invalidId = Guid.Empty;
            var request = new GetUsersDetailsQuery { UserIds = new List<Guid> { invalidId } };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Invalid user ID: {invalidId}", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNoUsersFound()
        {
            // Arrange
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var userIdObjects = userIds.Select(id => UserId.Create(id).Value).ToList();

            _unitOfWorkMock.Setup(uow => uow.Users.GetUsersByIds(It.IsAny<List<UserId>>()))
                .ReturnsAsync(new List<User>());

            var request = new GetUsersDetailsQuery { UserIds = userIds };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("No users found.", result.Error);
        }
    }
}
