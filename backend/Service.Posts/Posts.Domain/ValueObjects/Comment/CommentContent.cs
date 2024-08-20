using CSharpFunctionalExtensions;

namespace Posts.Domain.ValueObjects
{
    public class CommentContent : ValueObject
    {
        public string Content { get; }

        private CommentContent(string content)
        {
            Content = content;
        }

        public static Result<CommentContent> Create(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<CommentContent>("Comment content cannot be empty.");

            return Result.Success(new CommentContent(content));
        }

        public static implicit operator string(CommentContent commentContent)
        {
            return commentContent.Content;
        }

        public override string ToString()
        {
            return Content;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Content;
        }

        public static CommentContent Empty
        {
            get
            {
                return new CommentContent(string.Empty);
            }
        }
    }
}