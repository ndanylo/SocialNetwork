using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Notifications.Domain.ValueObjects;

namespace Notifications.Infrastructure.EF.ValueConverters
{
    public class PostIdValueConverter : ValueConverter<PostId, Guid>
    {
        public PostIdValueConverter() : base(
            postId => postId.Id,
            guid => PostId.Create(guid).Value)
        { }
    }
}