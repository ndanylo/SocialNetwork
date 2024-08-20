using Chats.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chats.EF.Configurations
{
    public class UserIdValueConverter : ValueConverter<UserId, Guid>
    {
        public UserIdValueConverter() : base(
            userId => userId.Id,
            guid => UserId.Create(guid).Value)
        { }
    }
}