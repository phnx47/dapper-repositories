using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace MicroOrm.Dapper.Repositories.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _reflectionPropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] FindClassProperties(this Type objectType)
        {
            if (_reflectionPropertyCache.ContainsKey(objectType))
                return _reflectionPropertyCache[objectType];

            var propertyInfos = objectType.GetProperties()
                .OrderBy(p => p.GetCustomAttributes<ColumnAttribute>()
                    .Select(a => a.Order)
                    .DefaultIfEmpty(int.MaxValue)
                    .FirstOrDefault()).ToArray();

            _reflectionPropertyCache.TryAdd(objectType, propertyInfos);

            return propertyInfos;
        }

        public static bool IsGenericType(this Type type)
        {
#if NESTANDARD13

            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if NESTANDARD13
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if NESTANDARD13
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        public static bool IsBool(this Type type)
        {
            return type == typeof(bool);
        }
    }
}
