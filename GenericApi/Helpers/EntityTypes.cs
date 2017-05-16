using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{
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



}