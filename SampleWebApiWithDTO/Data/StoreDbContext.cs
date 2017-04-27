using Microsoft.EntityFrameworkCore;
using SampleWebApiWithDTO.Models;

namespace SampleWebApiWithDTO
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
