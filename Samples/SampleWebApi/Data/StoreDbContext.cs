using GenericApi;
using Microsoft.EntityFrameworkCore;
using SampleWebApi.Models;

namespace SampleWebApi
{
    public class StoreDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public StoreDbContext()
        { }

        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options){ }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().Ignore(m => m.GenericApiState);
            builder.Entity<Order>().Ignore(m => m.GenericApiState);

            base.OnModelCreating(builder);
        }

    }
}
