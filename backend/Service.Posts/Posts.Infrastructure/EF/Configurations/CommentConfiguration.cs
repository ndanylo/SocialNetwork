using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;
using Posts.EF.ValueConverters;

namespace Posts.EF.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(new CommentIdValueConverter())
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(e => e.PostId)
                .HasConversion(new PostIdValueConverter())
                .IsRequired();

            builder.Property(e => e.CreatedAt).IsRequired();

            builder.OwnsOne(e => e.Content, b =>
            {
                b.Property(v => v.Content)
                    .HasColumnName("Content")
                    .IsRequired();
            });
        }
    }
}