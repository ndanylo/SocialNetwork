using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Users.Application.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<Result<List<UserViewModel>>>
    {
        public Guid UserId { get; set; }
    }
}
