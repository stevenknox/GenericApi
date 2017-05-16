using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{

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

}