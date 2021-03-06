using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GenericApi
{
    public class GenericDbContext : DbContext
    {
        public GenericDbContext()
        { }

        public GenericDbContext(DbContextOptions<GenericDbContext> options)
            : base(options)
        { }

    }

}
