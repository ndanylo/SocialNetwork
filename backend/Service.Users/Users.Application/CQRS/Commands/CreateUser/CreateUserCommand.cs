using MediatR;
using Microsoft.AspNetCore.Http;
using CSharpFunctionalExtensions;

namespace Users.Application.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Result<bool>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}
