using Authorization.Domain.ValueObjects;
using Authorization.Infrastructure.EF.ValueConverters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.EF.Configurations
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<UserId>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<UserId>> builder)
        {
            builder.ToTable("Roles");

            builder.Property(r => r.Id)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(r => r.Name).HasMaxLength(256);
            builder.Property(r => r.NormalizedName).HasMaxLength(256);
            builder.HasIndex(r => r.NormalizedName).IsUnique().HasDatabaseName("RoleNameIndex");
            builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

            builder.HasMany<IdentityUserRole<UserId>>()
                   .WithOne()
                   .HasForeignKey(ur => ur.RoleId)
                   .IsRequired();
        }
    }
}