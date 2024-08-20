using MediatR;
using Authorization.Domain.Abstractions;
using Authorization.Domain.Entities;
using CSharpFunctionalExtensions;
using Authorization.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Authorization.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IUserCredentialsRepository userCredentialsRepository, ILogger<RegisterUserCommandHandler> logger)
        {
            _userCredentialsRepository = userCredentialsRepository;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            Guid userGuid = Guid.NewGuid();
            var userIdResult = UserId.Create(userGuid);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<Guid>("Invalid user ID.");
            }

            var userResult = UserCredentials.Create(request.Email, userIdResult.Value);
            if (userResult.IsFailure)
            {
                return Result.Failure<Guid>(userResult.Error);
            }

            var user = userResult.Value;
            var registerResult = await _userCredentialsRepository.RegisterAsync(user, request.Password);
            if(registerResult.IsFailure)
            {
                return Result.Failure<Guid>(registerResult.Error);
            }
            
            var register = registerResult.Value;
            if (!register.Succeeded)
            {
                var errors = string.Join(", ", register.Errors.Select(e => e.Description));
                _logger.LogError("Failed to register user: {Errors}", errors);
                return Result.Failure<Guid>(errors);
            }
            _logger.LogInformation("User successfully registered with ID: {UserId}", userGuid);
            return Result.Success(userGuid);
        }
    }
}