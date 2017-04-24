using System;
using GenericApi.Tests.Model;
using Microsoft.EntityFrameworkCore;

namespace GenericApi.Tests
{
    public class SampleContext : DbContext
    {
        public DbSet<SampleEntity> SampleEntities { get; set; }
        public DbSet<SampleEntityWithGuid> SampleEntitiesWithGuid { get; set; }

        public SampleContext()
        { }

        public SampleContext(DbContextOptions<SampleContext> options)
            : base(options)
        { }
    }
}
