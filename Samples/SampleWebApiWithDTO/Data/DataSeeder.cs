using System.Linq;
using SampleWebApiWithDTO.Models;

namespace SampleWebApiWithDTO.Data
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

            var types = new ProductType[]
            {
                new ProductType { Name = "Food"  }
            };

            foreach (ProductType s in types)
            {
                context.ProductTypes.Add(s);
            }
            context.SaveChanges();

            var products = new Product[]
            {
                new Product{Name="Apples", Cost=10, ProductTypeId=1},
                new Product{Name="Oranges", Cost=5, ProductTypeId=1},
                new Product{Name="Grapes", Cost=15, ProductTypeId=1}
            };
            foreach (Product s in products)
            {
                context.Products.Add(s);
            }
            context.SaveChanges();

            var orders = new Order[]
          {
                new Order{ProductId = 1, Quantity = 2, Total = 20 },
                new Order{ProductId = 2, Quantity = 1, Total = 5 },
                new Order{ProductId = 3, Quantity = 4, Total = 60 },
          };
            foreach (Order s in orders)
            {
                context.Orders.Add(s);
            }
            context.SaveChanges();

        }
    }
}