using CSharpFunctionalExtensions;
using Users.Domain.Abstractions;
using Users.Infrastructure.EF;

namespace OnlineChat.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _context;

        public UnitOfWork(UserDbContext context,
                        IUserRepository userRepository,
                        IFriendRequestRepository friendRequestRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            FriendRequests = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
        }

        public IUserRepository Users { get; private set; }
        public IFriendRequestRepository FriendRequests { get; private set; }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
