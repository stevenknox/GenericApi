using System;
using System.Linq;
using GenericApi.Tests.Model;
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
        public void GenericService_Should_SaveEntity()
        {
            var data = $"New Entity from {nameof(GenericService_Should_SaveEntity)} ";
            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntity, SampleContext>(context);

                service.Add(new SampleEntity { Name = data });
            }

            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.SampleEntities.FirstOrDefault(f => f.Name == data));
            }
        }

       

        [Fact]
        public void GenericController_Should_CreateEntity()
        {
            var data = $"New Entity from {nameof(GenericController_Should_CreateEntity)} ";
            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, SampleContext>(service);

                controller.Post(new SampleEntity { Name = data });
            }

            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.SampleEntities.FirstOrDefault(f => f.Name == data));
            }
        }

        [Fact]
        public void GenericController_Should_UpdateEntity()
        {
            var data = $"Initial Data for {nameof(GenericController_Should_UpdateEntity)}";
            var updatedData = $"Updated Data for {nameof(GenericController_Should_UpdateEntity)}";
            var sut = SaveEntity(options, new SampleEntity { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, SampleContext>(service);

                //make our changes
                sut.Name = updatedData;

                controller.Put(sut.Id, sut);
            }

            using (var context = new SampleContext(options))
            {
                Assert.Same(context.SampleEntities.FirstOrDefault(f => f.Id == sut.Id).Name, updatedData);
            }
        }

        [Fact]
        public void GenericController_Should_DeleteEntity()
        {
            var data = $"Initial Data for {nameof(GenericController_Should_DeleteEntity)}";
            var sut = SaveEntity(options, new SampleEntity { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, SampleContext>(service);

                controller.Delete(sut.Id);
            }

            using (var context = new SampleContext(options))
            {
                Assert.Null(context.SampleEntities.FirstOrDefault(f => f.Id == sut.Id));
            }
        }

        private static SampleEntity SaveEntity(DbContextOptions<SampleContext> options, SampleEntity data)
        {
            using (var context = new SampleContext(options))
            {
                context.SampleEntities.Add(data);
                context.SaveChanges();
                return data;

            }
        }
    }
}
