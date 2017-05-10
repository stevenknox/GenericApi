﻿using GenericApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SampleWebApi.Data;
using System;

namespace SampleWebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<StoreDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //Use if EF Entities and DbContext in same assembly and only 1 DbContext exsits in that assembly
            services.
                AddMvc().
                AddGenericControllers(nameof(SampleWebApi));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SecureGenericApi", policy => policy.Requirements.Add(new SecureGenericApiRequirement(ApiAuthorization.Authorize)));
            });


            //Use if EF Entities and DbContext in same assembly but more than 1 DbContext exsits in that assembly
            //services.
            //   AddMvc().
            //   AddGenericControllers(nameof(SampleWebApi), typeof(StoreDbContext));

            services.AddGenericServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, StoreDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DataSeeder.Initialize(context);
        }

    }
}