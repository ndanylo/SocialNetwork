using CSharpFunctionalExtensions;
using Posts.Domain.ValueObjects;

namespace Posts.Domain.Entities
{
    public class Comment : Entity<CommentId>
    {
        public CommentContent Content { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public UserId UserId { get; private set; }
        public PostId PostId { get; private set; }

        private Comment()
        {
            Content = CommentContent.Empty;
            PostId = PostId.Empty;
            UserId = UserId.Empty;
        }

        private Comment(CommentId id,
                        UserId userId,
                        PostId postId,
                        CommentContent content) : base(id)
        {
            Id = id;
            UserId = userId;
            PostId = postId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Comment> Create(CommentId id,
                                            UserId userId,
                                            PostId postId,
                                            CommentContent content)
        {
            if (id == CommentId.Empty)
                return Result.Failure<Comment>("Comment ID cannot be null or empty.");

            if (userId == UserId.Empty)
                return Result.Failure<Comment>("User ID cannot be null or empty.");

            if (postId == PostId.Empty)
                return Result.Failure<Comment>("Post ID cannot be null or empty.");

            if (content == CommentContent.Empty)
                return Result.Failure<Comment>("Comment content cannot be null or empty.");

            var comment = new Comment(id, userId, postId, content);
            return Result.Success(comment);
        }

        public static Comment Default
        {
            get
            {
                return new Comment
                {
                    Content = CommentContent.Empty,
                    UserId = UserId.Empty,
                    PostId = PostId.Empty,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }
    }
}