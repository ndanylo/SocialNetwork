using CSharpFunctionalExtensions;

namespace Posts.Domain.ValueObjects
{
    public class PostContent : ValueObject
    {
        public string Content { get; private set; }

        private PostContent(string content)
        {
            Content = content;
        }

        public static Result<PostContent> Create(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<PostContent>("content cannot be empty.");

            return Result.Success(new PostContent(content));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Content;
        }

        public static implicit operator string(PostContent postContent)
        {
            return postContent.Content;
        }

        public override string ToString()
        {
            return Content;
        }

        public static PostContent Empty
        {
            get
            {
                return new PostContent(string.Empty);
            }
        }
    }
}