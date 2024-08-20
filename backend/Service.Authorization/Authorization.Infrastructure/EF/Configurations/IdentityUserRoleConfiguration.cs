using Authorization.Domain.ValueObjects;
using Authorization.Infrastructure.EF.ValueConverters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.EF.Configurations
{
    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<UserId>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<UserId>> builder)
        {
            builder.ToTable("UserRoles");

            builder.Property(ur => ur.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();
        }
    }
}