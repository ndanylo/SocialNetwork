using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using Users.Infrastructure.EF.Configurations;

namespace Users.Infrastructure.EF
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new FriendRequestConfiguration());
        }
    }
}
