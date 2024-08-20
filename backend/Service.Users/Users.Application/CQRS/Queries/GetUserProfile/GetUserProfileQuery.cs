using MediatR;
using CSharpFunctionalExtensions;
using Users.Application.ViewModels;

namespace Users.Application.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<Result<UserProfileViewModel>>
    {
        public Guid UserId { get; set; }
        public Guid ProfileUserId { get; set; }
    }
}