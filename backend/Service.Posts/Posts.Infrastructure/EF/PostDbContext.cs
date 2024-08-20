using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;
using Posts.EF.Configurations;

namespace Posts.Infrastructure.EF
{
    public class PostDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new PostConfiguration());
            builder.ApplyConfiguration(new LikeConfiguration());
            builder.ApplyConfiguration(new CommentConfiguration());
        }
    }
}