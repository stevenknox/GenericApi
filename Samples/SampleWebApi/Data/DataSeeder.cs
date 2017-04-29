using System.Linq;
using SampleWebApi.Models;

namespace SampleWebApi.Data
{
    public static class DataSeeder
    {
        public static void Initialize(StoreDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var entities = new Product[]
            {
                new Product{Name="Apples"},
                new Product{Name="Oranges"},
                new Product{Name="Grapes"}
            };
            foreach (Product s in entities)
            {
                context.Products.Add(s);
            }
            context.SaveChanges();

        }
    }
}