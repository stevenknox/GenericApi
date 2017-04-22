using System;
using System.Linq;
using GenericApi.Tests.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GenericApi.Tests
{
    public class GenericServiceTests
    {
        public GenericServiceTests()
        {
           
        }
        [Fact]
        public void GenericService_Should_SaveEntity()
        {
            var options = new DbContextOptionsBuilder<SampleContext>()
              .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
              .Options;

            var data = "New Entity";
            // Run the test against one instance of the context
            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntity, SampleContext>(context);

                service.Add(new SampleEntity { Name = data });
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.SampleEntities.FirstOrDefault(f=> f.Name == data));
            }
        }

        [Fact]
        public void GenericController_Should_SaveEntity()
        {
            var options = new DbContextOptionsBuilder<SampleContext>()
              .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
              .Options;

            var data = "New Entity from Controller";
            // Run the test against one instance of the context
            using (var context = new SampleContext(options))
            {
                var service = new GenericService<SampleEntity, SampleContext>(context);
                var controller = new GenericServiceController<SampleEntity, SampleContext>(service);

                service.Add(new SampleEntity { Name = data });
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new SampleContext(options))
            {
                Assert.NotNull(context.SampleEntities.FirstOrDefault(f => f.Name == data));
            }
        }
    }
}
