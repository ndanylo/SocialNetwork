using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notifications.Domain.Entities;
using Notifications.Infrastructure.EF.ValueConverters;

namespace Notifications.Infrastructure.EF.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(new NotificationIdValueConverter())
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(e => e.PostId)
                .HasConversion(new PostIdValueConverter())
                .IsRequired();

            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.IsRead).IsRequired();

            builder.OwnsOne(e => e.Content, b =>
            {
                b.Property(v => v.Content)
                    .HasColumnName("Content")
                    .IsRequired();
            });
        }
    }
}