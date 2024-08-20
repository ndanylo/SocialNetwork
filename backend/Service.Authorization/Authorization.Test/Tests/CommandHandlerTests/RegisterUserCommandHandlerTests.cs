using Authorization.Application.Commands.RegisterUser;
using Authorization.Domain.Abstractions;
using Authorization.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Authorization.Tests.Application.Commands
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserCredentialsRepository> _mockRepository;
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _mockLogger;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _mockRepository = new Mock<IUserCredentialsRepository>();
            _mockLogger = new Mock<ILogger<RegisterUserCommandHandler>>();
            _handler = new RegisterUserCommandHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "test@example.com", Password = "password123" };
            _mockRepository.Setup(r => r.RegisterAsync(It.IsAny<UserCredentials>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            _mockRepository.Verify(r => r.RegisterAsync(It.IsAny<UserCredentials>(), command.Password), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidEmail_ReturnsFailureResult()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "", Password = "password123" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("email can not be null or empty", result.Error);
        }

        [Fact]
        public async Task Handle_RegistrationFails_ReturnsFailureResult()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "test@example.com", Password = "password123" };
            var identityErrors = new List<IdentityError> { new IdentityError { Description = "Registration failed" } };
            _mockRepository.Setup(r => r.RegisterAsync(It.IsAny<UserCredentials>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Registration failed", result.Error);
        }
    }
}