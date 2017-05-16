using System;
using Microsoft.Extensions.DependencyInjection;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{
    public static class GenericControllersMiddleware
    {   
        public static void AddGenericControllers(this IMvcBuilder builder, string assemblyName, Type db = null)
        {
            var options = new Options { db = db, EntityAssemblyName = assemblyName };
            builder.ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new GenericControllerFeatureProvider(options)));
        }

        public static void AddGenericControllers(this IMvcBuilder builder, Options options)
        {
            builder.ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new GenericControllerFeatureProvider(options)));
        }


    }



}