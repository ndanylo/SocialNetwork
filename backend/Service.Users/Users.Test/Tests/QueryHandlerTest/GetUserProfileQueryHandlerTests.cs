using AutoMapper;
using CSharpFunctionalExtensions;
using Moq;
using Users.Application.Queries.GetUserProfile;
using Users.Application.Services.Abstractions;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Xunit;
using OnlineChat.Application.Users.Queries.GetUserProfile;

namespace Users.Application.Tests.Queries
{
    public class GetUserProfileQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPostService> _postServiceMock;
        private readonly Mock<ILogger<GetUserProfileQueryHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetUserProfileQueryHandler _handler;

        public GetUserProfileQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _postServiceMock = new Mock<IPostService>();
            _loggerMock = new Mock<ILogger<GetUserProfileQueryHandler>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetUserProfileQueryHandler(_mapperMock.Object,
                                                      _unitOfWorkMock.Object,
                                                      _postServiceMock.Object,
                                                      _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            // Arrange
            var request = new GetUserProfileQuery
            {
                UserId = Guid.Empty,
                ProfileUserId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenProfileUserIdIsInvalid()
        {
            // Arrange
            var request = new GetUserProfileQuery
            {
                UserId = Guid.NewGuid(),
                ProfileUserId = Guid.Empty
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid profile user ID.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var request = new GetUserProfileQuery
            {
                UserId = Guid.NewGuid(),
                ProfileUserId = Guid.NewGuid()
            };

            var profileUserId = UserId.Create(request.ProfileUserId).Value;

            _unitOfWorkMock
                .Setup(uow => uow.Users.GetUserByIdAsync(profileUserId))
                .ReturnsAsync(Result.Failure<User?>("User not found.")); // Simulate user not found

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains($"User with ID", result.Error);
        }
    }
}
