using System;
using System.Collections.Generic;
using System.Linq;
using GenericApi.Tests.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GenericApi.Tests
{
    public class GenericServiceTests
    {
        private DbContextOptions<SampleContext> options;
        public GenericServiceTests()
        {
              options = new DbContextOptionsBuilder<SampleContext>()
             .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
             .Options;
        }

        [Fact]
        public void GenericService_Should_GetEntities()
        {
            ResetData();
            var entities = SeedData(nameof(GenericService_Should_GetEntities));
            List<Blog> sut;
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);

                sut = service.GetAll();
            }

            Assert.Equal(10, sut.Count());
            
        }

        [Fact]
        public void GenericService_Should_SaveEntity()
        {
            var data = $"New Entity from {nameof(GenericService_Should_SaveEntity)} ";
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);

                service.Add(new Blog { Name = data });
            }

            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.Blogs.FirstOrDefault(f => f.Name == data));
            }
        }


        [Fact]
        public void GenericService_Should_SaveEntityGraph()
        {
            var data = $"New Entity from {nameof(GenericService_Should_SaveEntityGraph)} ";
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);

                var blog = new Blog { Name = data };
                blog.Posts.Add(new Post { Blog = blog, Content = "Blog Post", Title = "Post Title" });
                service.Add(blog);
            }

            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.Blogs.FirstOrDefault(f => f.Name == data));
                Assert.True(context.Blogs.Include(p => p.Posts).FirstOrDefault(f => f.Name == data).Posts.Count() > 0);
            }
        }


        [Fact]
        public void GenericController_Should_GetEntities()
        {
            ResetData();
            var entities = SeedData(nameof(GenericService_Should_GetEntities));
            List<Blog> sut;
          
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, int, SampleContext>(service);

                var response = controller.Get() as OkObjectResult;
                sut = response.Value as List<Blog>;
            }

            Assert.Equal(10, sut.Count());

        }

        [Fact]
        public void GenericController_Should_GetSingleEntity()
        {
            ResetData();
            var entity = SeedData(nameof(GenericController_Should_GetSingleEntity)).First();
            Blog sut;

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, int, SampleContext>(service);

                var response = controller.Find(entity.Id.ToString()) as OkObjectResult;
                sut = response.Value as Blog;
            }

            Assert.Equal(entity.Id, sut.Id);
            Assert.Equal(entity.Name, sut.Name);

        }


        [Fact]
        public void GenericController_Should_CreateEntity()
        {
            var data = $"New Entity from {nameof(GenericController_Should_CreateEntity)} ";
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, int, SampleContext>(service);

                controller.Post(new Blog { Name = data });
            }

            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.Blogs.FirstOrDefault(f => f.Name == data));
            }
        }

        [Fact]
        public void GenericController_Should_UpdateEntity()
        {
            var data = $"Initial Data for {nameof(GenericController_Should_UpdateEntity)}";
            var updatedData = $"Updated Data for {nameof(GenericController_Should_UpdateEntity)}";
            var sut = SaveEntity(options, new Blog { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, int, SampleContext>(service);

                //make our changes
                sut.Name = updatedData;

                controller.Put(sut.Id.ToString(), sut);
            }

            using (var context = new SampleContext(options))
            {
                Assert.Same(context.Blogs.FirstOrDefault(f => f.Id == sut.Id).Name, updatedData);
            }
        }

        [Fact]
        public void GenericController_Should_DeleteEntity()
        {
            var data = $"Initial Data for {nameof(GenericController_Should_DeleteEntity)}";
            var sut = SaveEntity(options, new Blog { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepositorySimple<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, int, SampleContext>(service);

                controller.Delete(sut.Id.ToString());
            }

            using (var context = new SampleContext(options))
            {
                Assert.Null(context.Blogs.FirstOrDefault(f => f.Id == sut.Id));
            }
        }

        [Fact]
        public void GenericController_Should_SupportGuidPrimaryKey()
        {
            var data = $"Initial Data for {nameof(GenericController_Should_SupportGuidPrimaryKey)}";
            var updatedData = $"Updated Data for {nameof(GenericController_Should_SupportGuidPrimaryKey)}";
            var sut = SaveGuidEntity(options, new EntityWithGuid { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<EntityWithGuid, Guid, SampleContext>(context);
                var controller = new GenericController<EntityWithGuid, Guid, SampleContext>(service);

                //make our changes
                sut.Name = updatedData;

                controller.Put(sut.Id.ToString(), sut);
            }

            using (var context = new SampleContext(options))
            {
                Assert.Same(context.EntitiesWithGuid.FirstOrDefault(f => f.Id == sut.Id).Name, updatedData);
            }
        }

        [Fact]
        public void GenericController_Should_GetSingleEntityWithGuid()
        {
            ResetData();
            var entity = SeedDataWithGuids(nameof(GenericController_Should_GetSingleEntityWithGuid)).First();
            EntityWithGuid sut;

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<EntityWithGuid, Guid, SampleContext>(context);
                var controller = new GenericController<EntityWithGuid, Guid, SampleContext>(service);

                var response = controller.Find(entity.Id.ToString()) as OkObjectResult;
                sut = response.Value as EntityWithGuid;
            }

            Assert.Equal(entity.Id, sut.Id);
            Assert.Equal(entity.Name, sut.Name);

        }

        private static Blog SaveEntity(DbContextOptions<SampleContext> options, Blog data)
        {
            using (var context = new SampleContext(options))
            {
                context.Blogs.Add(data);
                context.SaveChanges();
                return data;

            }
        }

        private static EntityWithGuid SaveGuidEntity(DbContextOptions<SampleContext> options, EntityWithGuid data)
        {
            using (var context = new SampleContext(options))
            {
                context.EntitiesWithGuid.Add(data);
                context.SaveChanges();
                return data;

            }
        }


        private List<Blog> SeedData(string name)
        {
            var entities = new List<Blog>();
            for (int i = 0; i < 10; i++)
            {
                entities.Add(SaveEntity(options, new Blog { Name = $"Entity {i} {name}" }));
            }
            return entities;
        }

        private List<EntityWithGuid> SeedDataWithGuids(string name)
        {
            var entities = new List<EntityWithGuid>();
            for (int i = 0; i < 10; i++)
            {
                entities.Add(SaveGuidEntity(options, new EntityWithGuid { Name = $"Entity {i} {name}" }));
            }
            return entities;
        }

        private void ResetData()
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
