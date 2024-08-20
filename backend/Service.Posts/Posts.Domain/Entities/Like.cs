using CSharpFunctionalExtensions;
using Posts.Domain.ValueObjects;

namespace Posts.Domain.Entities
{
    public class Like : Entity<LikeId>
    {
        public UserId UserId { get; private set; }
        public PostId PostId { get; private set; }

        private Like()
        {
            UserId = UserId.Empty;
            PostId = PostId.Empty;
        }

        private Like(LikeId id,
                    UserId userId,
                    PostId postId) : base(id)
        {
            Id = id;
            UserId = userId;
            PostId = postId;
        }

        public static Result<Like> Create(LikeId id,
                                        UserId userId,
                                        PostId postId)
        {
            if (id == LikeId.Empty)
                return Result.Failure<Like>("Like ID cannot be null or empty.");

            if (userId == UserId.Empty)
                return Result.Failure<Like>("User ID cannot be null or empty.");

            if (postId == PostId.Empty)
                return Result.Failure<Like>("Post ID cannot be null or empty.");

            var like = new Like(id, userId, postId);
            return Result.Success(like);
        }

        public static Like Default
        {
            get
            {
                return new Like
                {
                    UserId = UserId.Empty,
                    PostId = PostId.Empty
                };
            }
        }
    }
}