using Microsoft.EntityFrameworkCore;
using SampleWebApiWithMvcForms.Models;

namespace SampleWebApiWithMvcForms
{
    public class StoreDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Order> Orders { get; set; }

        public StoreDbContext()
        { }

        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        { }
    }
}
