using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Authorization.Domain.Entities;
using Authorization.EF.Configurations;
using Microsoft.AspNetCore.Identity;
using Authorization.Domain.ValueObjects;

namespace Authorization.Infrastructure.EF
{
    public class UserCredentialsDbContext : IdentityDbContext<UserCredentials, IdentityRole<UserId>, UserId>
    {
        public UserCredentialsDbContext(DbContextOptions<UserCredentialsDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserCredentialsConfiguration());
            builder.ApplyConfiguration(new IdentityUserRoleConfiguration());
            builder.ApplyConfiguration(new IdentityUserClaimConfiguration());
            builder.ApplyConfiguration(new IdentityUserLoginConfiguration());
            builder.ApplyConfiguration(new IdentityRoleConfiguration());
            builder.ApplyConfiguration(new IdentityUserTokenConfiguration());
        }
    }
}
