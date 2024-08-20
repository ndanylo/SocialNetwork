using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Notifications.Domain.ValueObjects;

namespace Notifications.Infrastructure.EF.ValueConverters
{
    public class NotificationIdValueConverter : ValueConverter<NotificationId, Guid>
    {
        public NotificationIdValueConverter() : base(
            notificationId => notificationId.Id,
            guid => NotificationId.Create(guid).Value)
        { }
    }
}