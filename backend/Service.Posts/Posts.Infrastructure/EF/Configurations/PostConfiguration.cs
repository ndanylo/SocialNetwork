using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;
using Posts.EF.ValueConverters;

namespace Posts.EF.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(new PostIdValueConverter())
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(e => e.CreatedAt).IsRequired();

            var likesNavigation = builder.Metadata.FindNavigation(nameof(Post.Likes));
            if (likesNavigation != null)
            {
                likesNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            }

            var commentsNavigation = builder.Metadata.FindNavigation(nameof(Post.Comments));
            if (commentsNavigation != null)
            {
                commentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            }

            builder.OwnsOne(e => e.Content, b =>
            {
                b.Property(v => v.Content)
                    .HasColumnName("Content")
                    .IsRequired();
            });

            builder.OwnsOne(e => e.Image, b =>
            {
                b.Property(v => v.Url)
                    .HasColumnName("PhotoUrl")
                    .IsRequired(false);
            });
        }
    }
}