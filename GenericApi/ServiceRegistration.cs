using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{
    public static class GenericServicesMiddleware
    {
        public static void AddGenericServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericService<,>), typeof(GenericServiceSimple<,>));
            services.AddScoped(typeof(IGenericService<,,>), typeof(GenericService<,,>));
        }

    }

    public static class GenericControllersMiddleware
    {   
        public static void AddGenericControllers(this IMvcBuilder builder, string assemblyName, Type db = null)
        {
            var options = new GenericControllerOptions { db = db, EntityAssemblyName = assemblyName, UseInputModels = false, UseViewModels = false };
            builder.ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new GenericControllerFeatureProvider(options)));
        }

        public static void AddGenericControllers(this IMvcBuilder builder, GenericControllerOptions options)
        {
            builder.ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new GenericControllerFeatureProvider(options)));
        }


    }

    public class EntityTypes
    {
        public static List<TypeInfo> GetTypesFromAssembly(Type type, string assemblyName)
        {
            List<Assembly> loadableAssemblies = Extensions.FindAssemblies(assemblyName);

            var typeList = new List<TypeInfo>();
            var types = loadableAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).ToList();

            types.ForEach(t => typeList.Add(t.GetTypeInfo()));

            return typeList;
        }

        
    }

  
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
     {
        public GenericControllerOptions Options { get; set; }
        public GenericControllerFeatureProvider(GenericControllerOptions options)
        {
            Options = options;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // This is designed to run after the default ControllerTypeProvider, so the list of 'real' controllers
            // has already been populated.
            if (Options.db == null)
                Options.db = EntityTypes.GetTypesFromAssembly(typeof(DbContext), Options.GetDbAssembly()).FirstOrDefault().AsType();

            foreach (var entityType in EntityTypes.GetTypesFromAssembly(typeof(IHasGenericService), Options.EntityAssemblyName))
            {
                var typeName = entityType.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {

                    var idType = entityType.GetPropertyType("Id");

                    // There's no 'real' controller for this entity, so add the generic version.
                    //here we scan for EntityViewModel and EntityInputModel and pass to our controller

                    var controllerType = typeof(GenericServiceController<,,>).MakeGenericType(entityType.AsType(), idType, Options.db).GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
     public class GenericControllerNameConvention : Attribute, IControllerModelConvention
    {
         public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() != typeof(GenericServiceController<,,>))
            {
                throw new Exception("Not a generic controller!");
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
        }
    }



}