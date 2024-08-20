using CSharpFunctionalExtensions;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Domain.Abstractions
{
    public interface IFriendRequestRepository
    {
        Task<Result<List<FriendRequest>>> GetReceivedFriendRequestsAsync(UserId userId);
        Task<Result<List<FriendRequest>>> GetSentFriendRequestsAsync(UserId userId);
        Task<Result> AddFriendRequestAsync(FriendRequest friendRequest);
        Task<Result<FriendRequest>> GetFriendRequestByIdAsync(FriendRequestId requestId);
        Result RemoveFriendRequest(FriendRequest friendRequest);
        Task<Result<FriendRequest?>> GetFriendRequestBySenderAndReceiverIdsAsync(UserId senderId, UserId receiverId);
        Task<Result<bool>> ExistsAsync(UserId senderId, UserId receiverId);
    }
}