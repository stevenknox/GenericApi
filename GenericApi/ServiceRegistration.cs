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
using Microsoft.Extensions.Options;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{
    public static class GenericServicesMiddleware
    {
        public static void AddGenericServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
            services.AddScoped(typeof(IGenericService<,,>), typeof(GenericServiceBase<,,>));
        }

    }

    public static class GenericControllersMiddleware
    {   
        public static void AddGenericControllers(this IMvcBuilder builder, string assemblyName, Type db)
        {
            builder.ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new GenericControllerFeatureProvider(assemblyName, db)));
        }

    }

    public class EntityTypes
    {
        public static List<TypeInfo> GetTypesFromAssembly(string assemblyName) {

            var loadableAssemblies = new List<Assembly>();

            var deps = DependencyContext.Default;
            foreach (var compilationLibrary in deps.CompileLibraries)
            {
                if (compilationLibrary.Name.ToLower().Contains(assemblyName.ToLower()))
                {
                    var assembly = Assembly.Load(new AssemblyName(compilationLibrary.Name));
                    loadableAssemblies.Add(assembly);
                }
            }

            var type = typeof(IHasGenericService);
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
        public string AssemblyName;
        public Type db;
        public GenericControllerFeatureProvider(string assemblyName, Type context)
        {
            db = context;
            AssemblyName = assemblyName;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // This is designed to run after the default ControllerTypeProvider, so the list of 'real' controllers
            // has already been populated.
            foreach (var entityType in EntityTypes.GetTypesFromAssembly(AssemblyName))
            {
                var typeName = entityType.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {
                    // There's no 'real' controller for this entity, so add the generic version.
                    var controllerType = typeof(GenericServiceController<,>).MakeGenericType(entityType.AsType(), db).GetTypeInfo();
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
            if (controller.ControllerType.GetGenericTypeDefinition() != typeof(GenericServiceController<,>))
            {
                throw new Exception("Not a generic controller!");
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
        }
    }

}