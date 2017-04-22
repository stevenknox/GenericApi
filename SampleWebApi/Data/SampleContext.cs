using Microsoft.EntityFrameworkCore;
using SampleWebApi.Models;

namespace SampleWebApi
{
    public class SampleContext : DbContext
    {
        public DbSet<SampleEntity> SampleEntities { get; set; }

        public SampleContext()
        { }

        public SampleContext(DbContextOptions<SampleContext> options)
            : base(options)
        { }
    }
}
