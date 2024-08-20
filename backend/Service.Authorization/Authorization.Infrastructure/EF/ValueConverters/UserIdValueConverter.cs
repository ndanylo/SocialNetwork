using Authorization.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Authorization.Infrastructure.EF.ValueConverters
{
    public class UserIdValueConverter : ValueConverter<UserId, Guid>
    {
        public UserIdValueConverter() : base(
            userId => userId.Id,
            guid => UserId.Create(guid).Value)
        { }
    }
}