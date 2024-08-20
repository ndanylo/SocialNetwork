using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Users.Application.Queries.GetUsersDetails
{
    public class GetUsersDetailsQuery : IRequest<Result<List<UserViewModel>>>
    {
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
}