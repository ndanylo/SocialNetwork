using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.EF.ValueConverters;

namespace Authorization.EF.Configurations
{
    public class UserCredentialsConfiguration : IEntityTypeConfiguration<UserCredentials>
    {
        public void Configure(EntityTypeBuilder<UserCredentials> builder)
        {

            builder.HasKey(e => e.Id);

            builder.Property(u => u.Id)
               .HasConversion(new UserIdValueConverter())
               .IsRequired();

        }
    }
}