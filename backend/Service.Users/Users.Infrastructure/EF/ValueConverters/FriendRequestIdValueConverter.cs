using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.EF.Configurations
{
    public class FriendRequestIdValueConverter : ValueConverter<FriendRequestId, Guid>
    {
        public FriendRequestIdValueConverter() : base(
            friendRequestId => friendRequestId.Id,
            guid => FriendRequestId.Create(guid).Value)
        { }
    }
}