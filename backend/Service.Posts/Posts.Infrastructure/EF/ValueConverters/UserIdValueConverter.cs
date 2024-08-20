using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Posts.Domain.ValueObjects;

namespace Posts.EF.ValueConverters
{
    public class UserIdValueConverter : ValueConverter<UserId, Guid>
    {
        public UserIdValueConverter() : base(
            userId => userId.Id,
            guid => UserId.Create(guid).Value)
        { }
    }
}