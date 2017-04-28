using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericApi
{
    public static class Helpers
    {
        public static List<Type> GetMapToEntityAttributes<T>()
        {
            var attrs = new List<Type>();

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                var att = prop.GetCustomAttributes(typeof(MapToEntityAttribute), false);
                if(att.Count() > 0)
                    attrs.Add((att.First() as MapToEntityAttribute).Name);
            }
                  
            return attrs;

        }
    }
}
