using CSharpFunctionalExtensions;

namespace Users.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IFriendRequestRepository FriendRequests { get; }
        Task<Result> SaveChangesAsync();
    }
}
