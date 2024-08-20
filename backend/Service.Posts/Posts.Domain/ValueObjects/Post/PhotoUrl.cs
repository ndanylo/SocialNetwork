using CSharpFunctionalExtensions;

namespace Posts.Domain.ValueObjects
{
    public class PhotoUrl : ValueObject
    {
        public string Url { get; }

        private PhotoUrl(string url)
        {
            Url = url;
        }

        public static Result<PhotoUrl> Create(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Result.Failure<PhotoUrl>("Photo URL cannot be empty.");

            return Result.Success(new PhotoUrl(url));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Url;
        }

        public static implicit operator string(PhotoUrl photoUrl)
        {
            return photoUrl.Url;
        }
    }
}