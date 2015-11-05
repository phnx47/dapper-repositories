using System;
using System.Reflection;

namespace MicroOrm.Dapper.Repositories.Extensions
{
    internal static class TypeExtensions
    {
        public static PropertyInfo GetProperty(this Type type, string name)
        {
#if COREFX
            return System.Reflection.TypeExtensions.GetProperty(type, name);
#else
            return type.GetProperty(name);
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if COREFX
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