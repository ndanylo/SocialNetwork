using CSharpFunctionalExtensions;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities
{
    public class User : Entity<UserId>
    {
        public List<User> Friends { get; private set; } = new List<User>();
        public List<FriendRequest> SentFriendRequests { get; private set; } = new List<FriendRequest>();
        public List<FriendRequest> ReceivedFriendRequests { get; private set; } = new List<FriendRequest>();

        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string? Avatar { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string City { get; private set; }

        private User()
        {
            Email = string.Empty;
            UserName = string.Empty;
            Avatar = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            City = string.Empty;
        }

        private User(UserId id,
                    string email,
                    string userName,
                    string firstName,
                    string lastName,
                    string city)
        {
            Id = id;
            Email = email;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            City = city;
        }

        public static Result<User> Create(UserId id,
                                        string email,
                                        string userName,
                                        string firstName,
                                        string lastName,
                                        string city)
        {
            if (id == UserId.Empty)
                return Result.Failure<User>("User ID cannot be empty.");

            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<User>("Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(userName))
                return Result.Failure<User>("Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<User>("First name cannot be empty.");

            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<User>("Last name cannot be empty.");

            if (string.IsNullOrWhiteSpace(city))
                return Result.Failure<User>("City cannot be empty.");

            var user = new User(id, email, userName, firstName, lastName, city);
            return Result.Success(user);
        }

        public Result AddFriend(User friend)
        {
            if (friend == null)
                return Result.Failure("Friend cannot be null.");

            if (Friends.Contains(friend))
                return Result.Failure("This user is already a friend.");

            Friends.Add(friend);
            return Result.Success();
        }

        public Result RemoveFriend(User friend)
        {
            if (friend == null)
                return Result.Failure("Friend cannot be null.");

            if (!Friends.Contains(friend))
                return Result.Failure("This user is not a friend.");

            Friends.Remove(friend);
            return Result.Success();
        }

        public Result AddSentFriendRequest(FriendRequest request)
        {
            if (request == null)
                return Result.Failure("Friend request cannot be null.");

            if (SentFriendRequests.Any(r => r.ReceiverId == request.ReceiverId))
                return Result.Failure("A friend request has already been sent to this user.");

            SentFriendRequests.Add(request);
            return Result.Success();
        }

        public Result AddReceivedFriendRequest(FriendRequest request)
        {
            if (request == null)
                return Result.Failure("Friend request cannot be null.");

            if (ReceivedFriendRequests.Any(r => r.SenderId == request.SenderId))
                return Result.Failure("A friend request has already been received from this user.");

            ReceivedFriendRequests.Add(request);
            return Result.Success();
        }

        public Result RemoveSentFriendRequest(FriendRequest request)
        {
            if (request == null)
                return Result.Failure("Friend request cannot be null.");

            if (!SentFriendRequests.Contains(request))
                return Result.Failure("This friend request was not sent.");

            SentFriendRequests.Remove(request);
            return Result.Success();
        }

        public Result RemoveReceivedFriendRequest(FriendRequest request)
        {
            if (request == null)
                return Result.Failure("Friend request cannot be null.");

            if (!ReceivedFriendRequests.Contains(request))
                return Result.Failure("This friend request was not received.");

            ReceivedFriendRequests.Remove(request);
            return Result.Success();
        }

        public void SetAvatar(string avatarPath)
        {
            Avatar = avatarPath;
        }

        public static User Default
        {
            get
            {
                return new User
                {
                    Avatar = string.Empty,
                    FirstName = "DefaultFirstName",
                    LastName = "DefaultLastName",
                    City = "DefaultCity"
                };
            }
        }
    }
}
