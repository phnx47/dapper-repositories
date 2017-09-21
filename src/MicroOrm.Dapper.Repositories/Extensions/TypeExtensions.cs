using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace MicroOrm.Dapper.Repositories.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> ReflectionPropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] FindClassProperties(this Type objectType)
        {
            if (ReflectionPropertyCache.ContainsKey(objectType))
                return ReflectionPropertyCache[objectType];

            var result = objectType.GetProperties().ToArray();

            ReflectionPropertyCache.TryAdd(objectType, result);

            return result;
        }


        public static bool IsGenericType(this Type type)
        {
#if NESTANDART13
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if NESTANDART13
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if NESTANDART13
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