using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
     public class GenericControllerNameConvention : Attribute, IControllerModelConvention
    {
         public void Apply(ControllerModel controller)
        {
            var x = controller.ControllerType.GetGenericTypeDefinition();
            if (x != typeof(GenericController<,>) && ! x.Name.Contains("DTOController`4") && x != typeof(MvcController<,>) && x.Name.Contains("MvcDTOController`4"))
            {
                throw new Exception("Not a generic controller!");
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
        }
    }



}