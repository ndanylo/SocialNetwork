using Moq;
using FluentAssertions;
using Authorization.Domain.Abstractions;
using OnlineChat.Application.Users.Commands.LoginUser;
using Authorization.Application.Services.Abstractions;
using Authorization.Domain.Entities;
using Authorization.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using CSharpFunctionalExtensions;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserCredentialsRepository> _userCredentialsRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userCredentialsRepositoryMock = new Mock<IUserCredentialsRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _handler = new LoginUserCommandHandler(_userCredentialsRepositoryMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenLoginIsSuccessful()
    {
        // Arrange
        var command = new LoginUserCommand
        {
            Email = "test@example.com",
            Password = "password"
        };
        var user = UserCredentials.Create("test@example.com", UserId.Create(Guid.NewGuid()).Value);
        var token = "generated_token";

        _userCredentialsRepositoryMock
            .Setup(repo => repo.LoginAsync(command.Email, command.Password))
            .ReturnsAsync(SignInResult.Success);

        _userCredentialsRepositoryMock
            .Setup(repo => repo.FindByEmailAsync(command.Email))
            .Returns(Task.FromResult(Result.Success<UserCredentials?>(user.Value)));

        _jwtServiceMock
            .Setup(service => service.GenerateToken(user.Value))
            .Returns(token);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(token);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLoginFails()
    {
        // Arrange
        var command = new LoginUserCommand
        {
            Email = "test@example.com",
            Password = "wrong_password"
        };

        _userCredentialsRepositoryMock
            .Setup(repo => repo.LoginAsync(command.Email, command.Password))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid username or password.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new LoginUserCommand
        {
            Email = "test@example.com",
            Password = "password"
        };

        _userCredentialsRepositoryMock
            .Setup(repo => repo.LoginAsync(command.Email, command.Password))
            .ReturnsAsync(SignInResult.Success);

        _userCredentialsRepositoryMock
            .Setup(repo => repo.FindByEmailAsync(command.Email))
            .ReturnsAsync((UserCredentials?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User is not found");
    }
}
