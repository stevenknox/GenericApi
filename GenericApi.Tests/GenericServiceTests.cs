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
            List<SampleEntity> sut;
            using (var context = new SampleContext(options))
            {
                var service = new GenericServiceSimple<SampleEntity, SampleContext>(context);

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
                var service = new GenericServiceSimple<SampleEntity, SampleContext>(context);

                service.Add(new SampleEntity { Name = data });
            }

            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.SampleEntities.FirstOrDefault(f => f.Name == data));
            }
        }


        [Fact]
        public void GenericController_Should_GetEntities()
        {
            ResetData();
            var entities = SeedData(nameof(GenericService_Should_GetEntities));
            List<SampleEntity> sut;
          
            using (var context = new SampleContext(options))
            {
                var service = new GenericServiceSimple<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, int, SampleContext>(service);

                var response = controller.Get() as OkObjectResult;
                sut = response.Value as List<SampleEntity>;
            }

            Assert.Equal(10, sut.Count());

        }

        [Fact]
        public void GenericController_Should_GetSingleEntity()
        {
            ResetData();
            var entity = SeedData(nameof(GenericController_Should_GetSingleEntity)).First();
            SampleEntity sut;

            using (var context = new SampleContext(options))
            {
                var service = new GenericServiceSimple<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, int, SampleContext>(service);

                var response = controller.Find(entity.Id.ToString()) as OkObjectResult;
                sut = response.Value as SampleEntity;
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
                var service = new GenericServiceSimple<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, int, SampleContext>(service);

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
                var service = new GenericServiceSimple<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, int, SampleContext>(service);

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
                var service = new GenericServiceSimple<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, int, SampleContext>(service);

                controller.Delete(sut.Id);
            }

            using (var context = new SampleContext(options))
            {
                Assert.Null(context.SampleEntities.FirstOrDefault(f => f.Id == sut.Id));
            }
        }

        [Fact]
        public void GenericController_Should_SupportGuidPrimaryKey()
        {
            var data = $"Initial Data for {nameof(GenericController_Should_SupportGuidPrimaryKey)}";
            var updatedData = $"Updated Data for {nameof(GenericController_Should_SupportGuidPrimaryKey)}";
            var sut = SaveGuidEntity(options, new SampleEntityWithGuid { Name = data });

            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntityWithGuid, Guid, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntityWithGuid, Guid, SampleContext>(service);

                //make our changes
                sut.Name = updatedData;

                controller.Put(sut.Id, sut);
            }

            using (var context = new SampleContext(options))
            {
                Assert.Same(context.SampleEntitiesWithGuid.FirstOrDefault(f => f.Id == sut.Id).Name, updatedData);
            }
        }

        [Fact]
        public void GenericController_Should_GetSingleEntityWithGuid()
        {
            ResetData();
            var entity = SeedDataWithGuids(nameof(GenericController_Should_GetSingleEntityWithGuid)).First();
            SampleEntityWithGuid sut;

            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntityWithGuid, Guid, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntityWithGuid, Guid, SampleContext>(service);

                var response = controller.Find(entity.Id.ToString()) as OkObjectResult;
                sut = response.Value as SampleEntityWithGuid;
            }

            Assert.Equal(entity.Id, sut.Id);
            Assert.Equal(entity.Name, sut.Name);

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

        private static SampleEntityWithGuid SaveGuidEntity(DbContextOptions<SampleContext> options, SampleEntityWithGuid data)
        {
            using (var context = new SampleContext(options))
            {
                context.SampleEntitiesWithGuid.Add(data);
                context.SaveChanges();
                return data;

            }
        }


        private List<SampleEntity> SeedData(string name)
        {
            var entities = new List<SampleEntity>();
            for (int i = 0; i < 10; i++)
            {
                entities.Add(SaveEntity(options, new SampleEntity { Name = $"Entity {i} {name}" }));
            }
            return entities;
        }

        private List<SampleEntityWithGuid> SeedDataWithGuids(string name)
        {
            var entities = new List<SampleEntityWithGuid>();
            for (int i = 0; i < 10; i++)
            {
                entities.Add(SaveGuidEntity(options, new SampleEntityWithGuid { Name = $"Entity {i} {name}" }));
            }
            return entities;
        }

        private void ResetData()
        {
            using (var context = new SampleContext(options))
            {
                context.SampleEntities.RemoveRange(context.SampleEntities);
                context.SampleEntitiesWithGuid.RemoveRange(context.SampleEntitiesWithGuid);
                context.SaveChanges();
            }
        }
    }
}
