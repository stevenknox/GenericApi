using System;
using GenericApi.Tests.Model;
using Microsoft.EntityFrameworkCore;

namespace GenericApi.Tests
{
    public class SampleContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<EntityWithGuid> EntitiesWithGuid { get; set; }

        public SampleContext()
        { }

        public SampleContext(DbContextOptions<SampleContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasMany(m => m.Posts)
                .WithOne(s => s.Blog);
        }
    }
}
