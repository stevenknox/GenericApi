using AutoMapper;
using GenericApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleWebApiWithDTO.Data;
using SampleWebApiWithDTO.Services;

namespace SampleWebApiWithDTO
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

            services.
               AddMvc().
                 AddGenericControllers(new OptionsBuilder
                 {
                     db = typeof(StoreDbContext),
                     DbContextAssemblyName = nameof(SampleWebApiWithDTO),
                     EntityAssemblyName = nameof(SampleWebApiWithDTO),
                     UseInputModels = true,
                     UseViewModels = true,
                 });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SecureGenericApi", policy => 
                policy.Requirements.Add(new SecureGenericApiRequirement(ApiAuthorization.AllowAnonymous)));
            });

            services.AddGenericServices(UseSanitizer: typeof(InputSanitizer));

            services.AddAutoMapper(typeof(Startup));
         
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
