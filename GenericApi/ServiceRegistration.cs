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
using Microsoft.AspNetCore.Authorization;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{
    public static class GenericServicesMiddleware
    {
        public static void AddGenericServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepositorySimple<,>));
            services.AddScoped(typeof(IGenericRepository<,,>), typeof(GenericRepository<,,>));
            services.AddSingleton<IAuthorizationHandler, SecureGenericApHandler>();
        }

    }

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

        public static Type GetTypeFromAssembly(string type, string assemblyName)
        {
            Assembly assembly = Extensions.FindAssemblies(assemblyName).First();

            return assembly.GetType(type);
        }


    }

  
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
     {
        public Options Options { get; set; }
        public GenericControllerFeatureProvider(Options options)
        {
            Options = options;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // This is designed to run after the default ControllerTypeProvider, so the list of 'real' controllers
            // has already been populated.
            bool EnableModelMapping = Options.HasProperty("UseViewModels") && Options.HasProperty("UseInputModels");
            bool EnableDTOMapping = Options.HasProperty("UseDTOs");

            if (Options.db == null)
                Options.db = EntityTypes.GetTypesFromAssembly(typeof(DbContext), Options.GetDbAssembly()).FirstOrDefault().AsType();

            foreach (var entityType in EntityTypes.GetTypesFromAssembly(typeof(IHasGenericRepository), Options.EntityAssemblyName))
            {
                var typeName = entityType.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {

                    var idType = entityType.GetPropertyType("Id");


                    //here we scan for EntityViewModel and EntityInputModel and pass to our controller


                    Type vm = null;
                    Type im = null;

                    //if Extensions package is loaded we should have extra properties
                    if (EnableModelMapping)
                    {
                        if (Convert.ToBoolean(Options.GetPropertyValue("UseViewModels")))
                            vm = EntityTypes.GetTypeFromAssembly(entityType.FullName + "ViewModel", Options.EntityAssemblyName);

                        if (Convert.ToBoolean(Options.GetPropertyValue("UseInputModels")))
                            im = EntityTypes.GetTypeFromAssembly(entityType.FullName + "InputModel", Options.EntityAssemblyName);
                    }
                    else if (EnableDTOMapping)
                    {
                        vm = EntityTypes.GetTypeFromAssembly(entityType.FullName + "DTO", Options.EntityAssemblyName);
                        im = vm;
                    }


                    if (vm == null) vm = entityType.AsType();
                    if (im == null) im = entityType.AsType();

                    // There's no 'real' controller for this entity, so add the generic version.

                    if (EnableModelMapping || EnableDTOMapping)
                    {
                        var dtoController = EntityTypes.GetTypeFromAssembly("GenericApi.DTOController`5", "GenericApi.ModelExtensions");
                        var controllerType = dtoController.MakeGenericType(entityType.AsType(), im, vm, idType, Options.db).GetTypeInfo();
                        feature.Controllers.Add(controllerType);
                    }
                    else
                    {
                        var controllerType = typeof(GenericController<,,>).MakeGenericType(entityType.AsType(), idType, Options.db).GetTypeInfo();
                        feature.Controllers.Add(controllerType);
                    }
                    
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
     public class GenericControllerNameConvention : Attribute, IControllerModelConvention
    {
         public void Apply(ControllerModel controller)
        {
            var x = controller.ControllerType.GetGenericTypeDefinition();
            if (x != typeof(GenericController<,,>) && ! x.Name.Contains("DTOController`5"))
            {
                throw new Exception("Not a generic controller!");
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
        }
    }



}