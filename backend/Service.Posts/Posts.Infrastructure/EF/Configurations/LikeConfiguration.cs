using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;
using Posts.EF.ValueConverters;

namespace Posts.EF.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(new LikeIdValueConverter())
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(e => e.PostId)
                .HasConversion(new PostIdValueConverter())
                .IsRequired();
        }
    }
}