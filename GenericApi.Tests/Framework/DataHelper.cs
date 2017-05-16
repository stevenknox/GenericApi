using GenericApi.Tests.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GenericApi.Tests.Framework
{
    public static class DataHelper
    {
        public static Blog SaveEntity(DbContextOptions<SampleContext> options, Blog data)
        {
            using (var context = new SampleContext(options))
            {
                context.Blogs.Add(data);
                context.SaveChanges();
                return data;

            }
        }

        public static EntityWithGuid SaveGuidEntity(DbContextOptions<SampleContext> options, EntityWithGuid data)
        {
            using (var context = new SampleContext(options))
            {
                context.EntitiesWithGuid.Add(data);
                context.SaveChanges();
                return data;

            }
        }


        public static List<Blog> SeedData(DbContextOptions<SampleContext> options, string name)
        {
            var entities = new List<Blog>();
            for (int i = 0; i < 10; i++)
            {
                entities.Add(SaveEntity(options, new Blog { Name = $"Entity {i} {name}" }));
            }
            return entities;
        }

        public static List<EntityWithGuid> SeedDataWithGuids(DbContextOptions<SampleContext> options, string name)
        {
            var entities = new List<EntityWithGuid>();
            for (int i = 0; i < 10; i++)
            {
                entities.Add(SaveGuidEntity(options, new EntityWithGuid { Name = $"Entity {i} {name}" }));
            }
            return entities;
        }

        public static void ResetData(DbContextOptions<SampleContext> options)
        {
            using (var context = new SampleContext(options))
            {
                context.Blogs.RemoveRange(context.Blogs);
                context.EntitiesWithGuid.RemoveRange(context.EntitiesWithGuid);
                context.SaveChanges();
            }
        }
    }
}
