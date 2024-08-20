using Authorization.Domain.ValueObjects;
using Authorization.Infrastructure.EF.ValueConverters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.EF.Configurations
{
    public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<UserId>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<UserId>> builder)
        {
            builder.ToTable("UserClaims");

            builder.Property(uc => uc.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(uc => uc.ClaimType).HasMaxLength(256);
            builder.Property(uc => uc.ClaimValue).HasMaxLength(256);
        }
    }
}