using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Posts.Domain.ValueObjects;

namespace Posts.EF.ValueConverters
{
    public class CommentIdValueConverter : ValueConverter<CommentId, Guid>
    {
        public CommentIdValueConverter() : base(
            commentId => commentId.Id,
            guid => CommentId.Create(guid).Value)
        { }
    }
}