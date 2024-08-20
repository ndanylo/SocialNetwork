using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Posts.Domain.ValueObjects;

namespace Posts.EF.ValueConverters
{
    public class LikeIdValueConverter : ValueConverter<LikeId, Guid>
    {
        public LikeIdValueConverter() : base(
            likeId => likeId.Id,
            guid => LikeId.Create(guid).Value)
        { }
    }
}