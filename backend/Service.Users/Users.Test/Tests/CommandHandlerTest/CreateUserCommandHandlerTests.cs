using FluentAssertions;
using Moq;
using Users.Application.Commands.CreateUser;
using Users.Domain.Entities;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Users.Application.Services.Abstractions;

namespace Users.Application.Tests
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _imageServiceMock = new Mock<IImageService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateUserCommandHandler(
                _identityServiceMock.Object,
                _imageServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenValid()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "test@example.com",
                Password = "Password123",
                FirstName = "John",
                LastName = "Doe",
                City = "New York",
                Avatar = FormFileHelper.CreateMockFormFile(new byte[] { 1, 2, 3 }) // Use the static helper
            };

            var registeredUserId = Guid.NewGuid();
            _identityServiceMock.Setup(s => s.RegisterUserAsync(command.Email, command.Password))
                .ReturnsAsync(Result.Success(registeredUserId));

            _imageServiceMock.Setup(s => s.SaveImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("avatar");

            var userIdResult = UserId.Create(registeredUserId);
            var userCreationResult = User.Create(
                userIdResult.Value,
                command.Email,
                command.Email,
                command.FirstName,
                command.LastName,
                command.City);

            if (userCreationResult.IsFailure)
            {
                throw new Exception(userCreationResult.Error);
            }

            var user = userCreationResult.Value;

            _unitOfWorkMock.Setup(u => u.Users.AddUserAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.Users.AddUserAsync(It.Is<User>(u =>
                u.Email == command.Email &&
                u.FirstName == command.FirstName &&
                u.LastName == command.LastName &&
                u.City == command.City &&
                u.Avatar == "avatar")), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRegisterUserFails()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "test@example.com",
                Password = "Password123",
                FirstName = "John",
                LastName = "Doe",
                City = "New York"
            };

            _identityServiceMock.Setup(s => s.RegisterUserAsync(command.Email, command.Password))
                .ReturnsAsync(Result.Failure<Guid>("Registration failed"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Registration failed");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserCreationFails()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "test@example.com",
                Password = "Password123",
                FirstName = "John",
                LastName = "Doe",
                City = "New York"
            };

            var registeredUserId = Guid.NewGuid();
            _identityServiceMock.Setup(s => s.RegisterUserAsync(command.Email, command.Password))
                .ReturnsAsync(Result.Success(registeredUserId));

            _unitOfWorkMock.Setup(u => u.Users.AddUserAsync(It.IsAny<User>()))
                .ReturnsAsync(Result.Failure<User>("Error adding user"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Error adding user");
        }
    }
}
