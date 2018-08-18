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
            DataHelper.ResetData(options);
            var entities = DataHelper.SeedData(options, nameof(GenericService_Should_GetEntities));
            List<Blog> sut;
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<Blog, SampleContext>(context);

                sut = service.GetAll();
            }

            Assert.Equal(10, sut.Count());

        }

        [Fact]
        public void GenericService_Should_SaveEntity()
        {
            DataHelper.ResetData(options);
            var data = $"New Entity from {nameof(GenericService_Should_SaveEntity)} ";
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<Blog, SampleContext>(context);

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
            DataHelper.ResetData(options);
            var data = $"New Entity from {nameof(GenericService_Should_SaveEntityGraph)} ";
            using (var context = new SampleContext(options))
            {
                var service = new GenericRepository<Blog, SampleContext>(context);

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

    }
}
