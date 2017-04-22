using System.Linq;
using SampleWebApi.Models;

namespace SampleWebApi.Data
{
    public static class SampleDataSeeder
    {
        public static void Initialize(SampleContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.SampleEntities.Any())
            {
                return;   // DB has been seeded
            }

            var entities = new SampleEntity[]
            {
                new SampleEntity{Name="Sample A"},
                new SampleEntity{Name="Sample B"},
                new SampleEntity{Name="Sample C"}
            };
            foreach (SampleEntity s in entities)
            {
                context.SampleEntities.Add(s);
            }
            context.SaveChanges();

        }
    }
}