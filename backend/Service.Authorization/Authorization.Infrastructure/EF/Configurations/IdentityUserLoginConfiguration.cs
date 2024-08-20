using Authorization.Domain.ValueObjects;
using Authorization.Infrastructure.EF.ValueConverters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.EF.Configurations
{
    public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<UserId>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<UserId>> builder)
        {
            builder.ToTable("UserLogins");

            builder.Property(ul => ul.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(ul => ul.LoginProvider).HasMaxLength(128);
            builder.Property(ul => ul.ProviderKey).HasMaxLength(128);
            builder.Property(ul => ul.ProviderDisplayName).HasMaxLength(256);
        }
    }
}