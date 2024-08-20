using CSharpFunctionalExtensions;
using Posts.Domain.ValueObjects;

namespace Posts.Domain.Entities
{
    public class Post : Entity<PostId>
    {
        public PostContent Content { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public UserId UserId { get; private set; }
        public PhotoUrl? Image { get; private set; }
        public List<Like> Likes { get; private set; } = new List<Like>();
        public List<Comment> Comments { get; private set; } = new List<Comment>();

        private Post()
        {
            Content = PostContent.Empty;
            UserId = UserId.Empty;
        }

        private Post(PostId id,
                    UserId userId,
                    PostContent content,
                    PhotoUrl image) : base(id)
        {
            Id = id;
            CreatedAt = DateTime.Now;
            UserId = userId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
            Image = image;
        }

        public static Result<Post> Create(PostId id,
                                        UserId userId,
                                        PostContent content,
                                        PhotoUrl image)
        {
            if (id == PostId.Empty)
                return Result.Failure<Post>("Post ID cannot be null or empty.");

            if (userId == UserId.Empty)
                return Result.Failure<Post>("User ID cannot be null or empty.");

            if (content == PostContent.Empty)
                return Result.Failure<Post>("Content cannot be null or empty.");

            if (image == null)
            {
                return Result.Failure<Post>("Photo url cannot be null.");
            }

            var post = new Post(id, userId, content, image);
            return Result.Success(post);
        }

        public void AddLike(Like like)
        {
            if (like == null)
                throw new ArgumentNullException(nameof(like));

            if (!Likes.Contains(like))
                Likes.Add(like);
        }

        public void Unlike(Like like)
        {
            if (like == null)
                throw new ArgumentNullException(nameof(like));

            Likes.Remove(like);
        }

        public void AddComment(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            Comments.Add(comment);
        }

        public bool DeleteComment(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            return Comments.Remove(comment);
        }

        public static Post Default
        {
            get
            {
                return new Post
                {
                    Content = PostContent.Empty,
                    UserId = UserId.Empty,
                    CreatedAt = DateTime.Now
                };
            }
        }
    }
}