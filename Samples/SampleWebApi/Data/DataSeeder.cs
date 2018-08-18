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

            var orders = new Order[]
            {
                new Order{ProductId=1, Quantity=5},
                new Order{ProductId=2, Quantity=1},
                new Order{ProductId=3, Quantity=4},
            };
            foreach (Order s in orders)
            {
                context.Orders.Add(s);
            }
            context.SaveChanges();

        }
    }
}