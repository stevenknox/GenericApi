using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleWebApi.Data;

namespace SampleWebApi
{
    public class Program
    {
      public static void Main(string[] args)
        {
            BuildWebHost(args)
            .MigrateDatabase()
            .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }

    //https://github.com/aspnet/EntityFramework/issues/9033
    public static class Ext
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var db = services.GetRequiredService<StoreDbContext>();
                     DataSeeder.Initialize(db);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

             return webHost;
        }
    }
    
}
