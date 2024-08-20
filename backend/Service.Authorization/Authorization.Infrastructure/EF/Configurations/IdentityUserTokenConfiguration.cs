using Authorization.Domain.ValueObjects;
using Authorization.Infrastructure.EF.ValueConverters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.EF.Configurations
{
    public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<UserId>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<UserId>> builder)
        {
            builder.ToTable("UserTokens");

            builder.Property(ut => ut.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(ut => ut.LoginProvider).HasMaxLength(128);
            builder.Property(ut => ut.Name).HasMaxLength(128);
        }
    }
}