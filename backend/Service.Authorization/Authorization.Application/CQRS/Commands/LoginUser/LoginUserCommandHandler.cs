using MediatR;
using CSharpFunctionalExtensions;
using Authorization.Domain.Abstractions;
using Authorization.Application.Services.Abstractions;

namespace OnlineChat.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string>>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository;
        private readonly IJwtService _jwtService;

        public LoginUserCommandHandler(IUserCredentialsRepository userCredentialsRepository, IJwtService jwtService)
        {
            _userCredentialsRepository = userCredentialsRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var signInResult = await _userCredentialsRepository.LoginAsync(request.Email, request.Password);
            if(signInResult.IsFailure)
            {
                return Result.Failure<string>(signInResult.Error);
            }

            var signIn = signInResult.Value;
            if (!signIn.Succeeded)
            {
                return Result.Failure<string>("Invalid username or password.");
            }

            var findUserResult = await _userCredentialsRepository.FindByEmailAsync(request.Email);
            if(findUserResult.IsFailure)
            {
                return Result.Failure<string>(findUserResult.Error);
            }

            var user = findUserResult.Value;
            if (user != null)
            {
                var token = _jwtService.GenerateToken(user);
                return Result.Success(token);
            }

            return Result.Failure<string>("User is not found");
        }
    }
}
