using GenericApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SampleWebApi.Data;
using System;
using SampleWebApi.Services;

namespace SampleWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<StoreDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //Use if EF Entities and DbContext in same assembly and only 1 DbContext exsits in that assembly
            services.
                AddMvc().
                AddGenericControllers(nameof(SampleWebApi));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SecureGenericApi", policy => policy.Requirements.Add(new SecureGenericApiRequirement(ApiAuthorization.AllowAnonymous)));
            });


            //Use if EF Entities and DbContext in same assembly but more than 1 DbContext exsits in that assembly
            //services.
            //   AddMvc().
            //   AddGenericControllers(nameof(SampleWebApi), typeof(StoreDbContext));

            services.AddGenericServices();

            //if you want to sanitize Post/Put models
            //services.AddTransient<IInputSanitizer, InputSanitizer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }

    }
}
