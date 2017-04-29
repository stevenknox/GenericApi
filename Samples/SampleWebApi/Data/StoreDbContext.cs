using Microsoft.EntityFrameworkCore;
using SampleWebApi.Models;

namespace SampleWebApi
{
    public class StoreDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public StoreDbContext()
        { }

        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        { }
    }
}
