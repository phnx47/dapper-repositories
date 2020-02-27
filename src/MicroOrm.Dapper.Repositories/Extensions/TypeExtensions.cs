using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _reflectionPropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _reflectionPrimitivePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] FindClassProperties(this Type objectType)
        {
            if (_reflectionPropertyCache.TryGetValue(objectType, out var cachedEntry))
                return cachedEntry;

            var propertyInfos = objectType.GetProperties()
                .OrderBy(p => p.GetCustomAttributes<ColumnAttribute>()
                    .Select(a => a.Order)
                    .DefaultIfEmpty(int.MaxValue)
                    .FirstOrDefault()).ToArray();

            _reflectionPropertyCache.TryAdd(objectType, propertyInfos);

            return propertyInfos;
        }

        public static PropertyInfo[] FindClassPrimitiveProperties(this Type objectType)
        {
            if (_reflectionPrimitivePropertyCache.TryGetValue(objectType, out var cachedEntry))
                return cachedEntry;

            var propertyInfos = objectType.GetProperties()
                .OrderBy(p => p.GetCustomAttributes<ColumnAttribute>()
                    .Select(a => a.Order)
                    .DefaultIfEmpty(int.MaxValue)
                    .FirstOrDefault()).Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            _reflectionPrimitivePropertyCache.TryAdd(objectType, propertyInfos);

            return propertyInfos;
        }
    }
}
