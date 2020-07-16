using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _reflectionPropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _reflectionPrimitivePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, SqlPropertyMetadata[]> _metaDataPropertyCache = new ConcurrentDictionary<Type, SqlPropertyMetadata[]>();

        public static PropertyInfo[] FindClassProperties(this Type objectType)
        {
            if (_reflectionPropertyCache.TryGetValue(objectType, out var cachedEntry))
                return cachedEntry;

            var propertyInfos = objectType.GetProperties()
                .OrderByDescending(x => x.GetCustomAttribute<IdentityAttribute>() != null)
                .ThenByDescending(x => x.GetCustomAttribute<KeyAttribute>() != null)
                .ThenBy(p => p.GetCustomAttributes<ColumnAttribute>()
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

            var props = objectType.GetProperties();
            var propertyInfos = props
                .OrderByDescending(x => x.GetCustomAttribute<IdentityAttribute>() != null)
                .ThenByDescending(x => x.GetCustomAttribute<KeyAttribute>() != null)
                .ThenBy(p => p.GetCustomAttributes<ColumnAttribute>()
                    .Select(a => a.Order)
                    .DefaultIfEmpty(int.MaxValue)
                    .FirstOrDefault()).Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            _reflectionPrimitivePropertyCache.TryAdd(objectType, propertyInfos);

            return propertyInfos;
        }

        public static SqlPropertyMetadata[] FindClassMetaDataProperties(this Type objectType)
        {
            if (_metaDataPropertyCache.TryGetValue(objectType, out var cachedEntry))
                return cachedEntry;

            var props = objectType.GetProperties();
            var propertyInfos = props
                .OrderByDescending(x => x.GetCustomAttribute<IdentityAttribute>() != null)
                .ThenByDescending(x => x.GetCustomAttribute<KeyAttribute>() != null)
                .ThenBy(p => p.GetCustomAttributes<ColumnAttribute>()
                    .Select(a => a.Order)
                    .DefaultIfEmpty(int.MaxValue)
                    .FirstOrDefault()).Where(ExpressionHelper.GetPrimitivePropertiesPredicate())
                .Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();

            _metaDataPropertyCache.TryAdd(objectType, propertyInfos);

            return propertyInfos;
        }

        public static Type UnwrapNullableType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;
    }
}
