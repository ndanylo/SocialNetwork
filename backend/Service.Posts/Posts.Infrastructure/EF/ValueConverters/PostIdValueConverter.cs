using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Posts.Domain.ValueObjects;

namespace Posts.EF.ValueConverters
{
    public class PostIdValueConverter : ValueConverter<PostId, Guid>
    {
        public PostIdValueConverter() : base(
            postId => postId.Id,
            guid => PostId.Create(guid).Value)
        { }
    }
}