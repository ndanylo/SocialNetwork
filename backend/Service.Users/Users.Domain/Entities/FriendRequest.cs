using CSharpFunctionalExtensions;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities
{
    public class FriendRequest : Entity<FriendRequestId>
    {
        public UserId SenderId { get; private set; }
        public virtual User Sender { get; private set; }
        public UserId ReceiverId { get; private set; }
        public virtual User Receiver { get; private set; }
        public DateTime RequestDate { get; private set; } = DateTime.Now;

        private FriendRequest()
        {
            SenderId = UserId.Empty;
            ReceiverId = UserId.Empty;
            Sender = User.Default;
            Receiver = User.Default;
        }

        private FriendRequest(FriendRequestId id,
                              UserId senderId,
                              UserId receiverId,
                              User sender,
                              User receiver) : base(id)
        {
            Id = id;
            Sender = sender;
            Receiver = receiver;
            SenderId = senderId;
            ReceiverId = receiverId;
            RequestDate = DateTime.UtcNow;
        }

        public static Result<FriendRequest> Create(FriendRequestId id,
                                                   UserId senderId,
                                                   UserId receiverId,
                                                   User sender,
                                                   User receiver)
        {
            if (id == FriendRequestId.Empty)
                return Result.Failure<FriendRequest>("Friend Request ID cannot be null or empty.");

            if (senderId == UserId.Empty)
                return Result.Failure<FriendRequest>("Sender ID cannot be null or empty.");

            if (receiverId == UserId.Empty)
                return Result.Failure<FriendRequest>("Receiver ID cannot be null or empty.");

            if (senderId.Equals(receiverId))
                return Result.Failure<FriendRequest>("Cannot send friend request to yourself.");

            var friendRequest = new FriendRequest(id, senderId, receiverId, sender, receiver);
            return Result.Success(friendRequest);
        }

        public static FriendRequest Default
        {
            get
            {
                return new FriendRequest
                {
                    SenderId = UserId.Empty,
                    ReceiverId = UserId.Empty,
                    RequestDate = DateTime.UtcNow
                };
            }
        }
    }
}
