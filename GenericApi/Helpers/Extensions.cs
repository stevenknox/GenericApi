using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace GenericApi
{
    public static class Extensions
    {
        public static bool IsPrimitive(this PropertyInfo info)
        {
            return info.GetType().GetTypeInfo().IsPrimitive;
        }

        public static JObject ToJObject(this object obj)
        {
            return obj as JObject;
        }

        public static Expression<Func<T, object>[]> MapIncludes<T>(this List<string> props)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var x = new List<Expression>();
            foreach (var prop in props)
            {
                Expression expression = Expression.Property(param, prop);
                x.Add(expression);

            }
            NewArrayExpression parent = Expression.NewArrayInit(typeof(T), x);
            var convert = Expression.Convert(parent, typeof(object[]));
            return Expression.Lambda<Func<T, object>[]>(convert, param);

        }

        public static Expression<Func<T, object>> MapIncludes<T>(this string property)
        {
            var param = Expression.Parameter(typeof(T), "p");

            Expression parent = Expression.Property(param, property);

            if (!parent.Type.GetTypeInfo().IsValueType)
            {
                return Expression.Lambda<Func<T, object>>(parent, param);
            }
            var convert = Expression.Convert(parent, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, param);
        }

        public static Expression<Func<T, object>> MapIncludes<T>(this Type property)
        {
            var param = Expression.Parameter(typeof(T), "p");

            Expression parent = Expression.Property(param, property.Name);

            if (!parent.Type.GetTypeInfo().IsValueType)
            {
                return Expression.Lambda<Func<T, object>>(parent, param);
            }
            var convert = Expression.Convert(parent, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, param);
        }

        public static List<Assembly> FindAssemblies(string assemblyName)
        {
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

            return loadableAssemblies;
        }

        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t));
        }

        public static object ConvertTo(this string id, Type targetType)
        {
            if (targetType == typeof(Guid))
            {
                return new Guid(id);
            }
            else
            {
                return Convert.ChangeType(id, targetType);
            }

        }

        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static Type GetPropertyType(this TypeInfo obj, String name)
        {
                if (obj == null) { return null; }

                PropertyInfo info = obj.GetProperty(name);
                if (info == null) { return null; }

                return info.GetType();
        }

        public static T GetPropertyType<T>(this TypeInfo obj, String name)
        {
            Object retval = GetPropertyType(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }

      
        public static void ApplyStateChanges(this DbContext context, GenericEntity entity, EntityState state)
        {
            context.ChangeTracker.TrackGraph(entity, e => e.Entry.State = GetEntityState(state));
        }
        
        public static Microsoft.EntityFrameworkCore.EntityState GetEntityState(EntityState entityState)
        {
            switch (entityState)
            {
                case EntityState.Unchanged:
                    return Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                case EntityState.Added:
                    return Microsoft.EntityFrameworkCore.EntityState.Added;
                case EntityState.Modified:
                    return Microsoft.EntityFrameworkCore.EntityState.Modified;
                case EntityState.Deleted:
                    return Microsoft.EntityFrameworkCore.EntityState.Deleted;
                default:
                    return Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }

    }
}
