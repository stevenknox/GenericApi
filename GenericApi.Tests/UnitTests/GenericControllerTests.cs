using System;
using System.Collections.Generic;
using System.Linq;
using GenericApi.Tests.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GenericApi.Tests.Framework;

namespace GenericApi.Tests
{
    public class GenericControllerTests
    {
        private DbContextOptions<SampleContext> options;

        public GenericControllerTests()
        {
            options = new DbContextOptionsBuilder<SampleContext>()
           .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
           .Options;
        }

        [Fact]
        public void GenericController_Should_GetEntities()
        {
            DataHelper.ResetData(options);
            var entities = DataHelper.SeedData(options, nameof(GenericController_Should_GetEntities));
            List<Blog> sut;

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, SampleContext>(service);

                var response = controller.Get() as OkObjectResult;
                sut = response.Value as List<Blog>;
            }

            Assert.Equal(10, sut.Count());

        }

        [Fact]
        public void GenericController_Should_GetSingleEntity()
        {
            DataHelper.ResetData(options);
            var entity = DataHelper.SeedData(options, nameof(GenericController_Should_GetSingleEntity)).First();
            Blog sut;

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, SampleContext>(service);

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
                var service = new GenericRepository<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, SampleContext>(service);

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
            var sut = DataHelper.SaveEntity(options, new Blog { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, SampleContext>(service);

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
            var sut = DataHelper.SaveEntity(options, new Blog { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<Blog, SampleContext>(context);
                var controller = new GenericController<Blog, SampleContext>(service);

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
            var sut = DataHelper.SaveGuidEntity(options, new EntityWithGuid { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<EntityWithGuid, SampleContext>(context);
                var controller = new GenericController<EntityWithGuid, SampleContext>(service);

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
            DataHelper.ResetData(options);
            var entity = DataHelper.SeedDataWithGuids(options, nameof(GenericController_Should_GetSingleEntityWithGuid)).First();
            EntityWithGuid sut;

            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<EntityWithGuid, SampleContext>(context);
                var controller = new GenericController<EntityWithGuid, SampleContext>(service);

                var response = controller.Find(entity.Id.ToString()) as OkObjectResult;
                sut = response.Value as EntityWithGuid;
            }

            Assert.Equal(entity.Id, sut.Id);
            Assert.Equal(entity.Name, sut.Name);

        }

    }
}
