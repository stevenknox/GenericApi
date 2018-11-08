using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace GenericApi
{
    public static class GenericServicesMiddleware
    {
        public static IServiceCollection AddGenericMvc(this IServiceCollection services)
        {
            var embeddedFileProvider = new EmbeddedFileProvider(typeof(FormFactory.FF).GetTypeInfo().Assembly,nameof(FormFactory)); 
            //Add the file provider to the Razor view engine
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(embeddedFileProvider);
            });

            return services;
        }

        public static IApplicationBuilder UseGenericMvc(this IApplicationBuilder app)
        {
           app.UseStaticFiles();
            {
                var options = new StaticFileOptions
                {
                    RequestPath = "",
                    FileProvider = new EmbeddedFileProvider(typeof(FormFactory.FF).GetTypeInfo().Assembly, nameof(FormFactory))
                };

                app.UseStaticFiles(options);
            }

            return app;
        }
    }
}