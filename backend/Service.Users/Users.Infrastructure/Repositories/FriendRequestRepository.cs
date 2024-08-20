
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Abstractions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using Users.Infrastructure.EF;

namespace OnlineChat.Infrastructure.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly UserDbContext _context;

        public FriendRequestRepository(UserDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<List<FriendRequest>>> GetReceivedFriendRequestsAsync(UserId userId)
        {
            try
            { 
                var requests = await _context.FriendRequests
                    .Include(fr => fr.Sender)
                    .Where(fr => fr.ReceiverId == userId)
                    .ToListAsync();

                return Result.Success(requests);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<FriendRequest>>(ex.Message);
            }
        }

        public async Task<Result<FriendRequest?>> GetFriendRequestBySenderAndReceiverIdsAsync(UserId senderId, UserId receiverId)
        {
            try
            {
                var friendRequest = await _context.FriendRequests
                    .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

                return Result.Success(friendRequest);
            }
            catch(Exception ex)
            {
                return Result.Failure<FriendRequest?>(ex.Message);
            }
        }

        public async Task<Result<List<FriendRequest>>> GetSentFriendRequestsAsync(UserId userId)
        {
            try
            {
                var requests = await _context.FriendRequests
                    .Include(fr => fr.Sender)
                    .Include(fr => fr.Receiver)
                    .Where(fr => fr.SenderId == userId)
                    .ToListAsync();

                return Result.Success(requests);
            }
            catch(Exception ex)
            {
                return Result.Failure<List<FriendRequest>>(ex.Message);
            }
        }

        public async Task<Result<bool>> ExistsAsync(UserId senderId, UserId receiverId)
        {
            try
            {
                var friendRequests = await _context.FriendRequests
                    .AnyAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

                return Result.Success(friendRequests);
            }
            catch(Exception ex)
            {
                return Result.Failure<bool>(ex.Message);
            }
        }

        public async Task<Result> AddFriendRequestAsync(FriendRequest friendRequest)
        {
            try
            {
               await _context.FriendRequests.AddAsync(friendRequest);

               return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure<Result>(ex.Message);
            }
        }

        public async Task<Result<FriendRequest>> GetFriendRequestByIdAsync(FriendRequestId requestId)
        {
            try
            {
                var friendRequest = await _context.FriendRequests.FindAsync(requestId);

                if (friendRequest == null)
                {
                    return Result.Failure<FriendRequest>("FriendRequest was not found.");
                }

                return Result.Success(friendRequest);
            }
            catch(Exception ex)
            {
                return Result.Failure<FriendRequest>(ex.Message);
            }
        }

        public Result RemoveFriendRequest(FriendRequest friendRequest)
        {
            try
            {
                _context.FriendRequests.Remove(friendRequest);
                return Result.Success();
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}