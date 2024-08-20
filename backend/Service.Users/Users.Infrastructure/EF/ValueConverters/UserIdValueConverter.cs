using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.EF.Configurations
{
    public class UserIdValueConverter : ValueConverter<UserId, Guid>
    {
        public UserIdValueConverter() : base(
            userId => userId.Id,
            guid => UserId.Create(guid).Value)
        { }
    }
}