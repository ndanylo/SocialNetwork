using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Notifications.Domain.ValueObjects;

namespace Notifications.Infrastructure.EF.ValueConverters
{
    public class UserIdValueConverter : ValueConverter<UserId, Guid>
    {
        public UserIdValueConverter() : base(
            userId => userId.Id,
            guid => UserId.Create(guid).Value)
        { }
    }
}