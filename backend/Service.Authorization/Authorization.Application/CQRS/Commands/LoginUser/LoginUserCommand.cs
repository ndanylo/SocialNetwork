using MediatR;
using CSharpFunctionalExtensions;

namespace OnlineChat.Application.Users.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<Result<string>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}