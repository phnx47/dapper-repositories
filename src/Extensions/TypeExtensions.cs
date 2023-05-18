using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories.Extensions;

internal static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _reflectionPropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _reflectionPrimitivePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
    private static readonly ConcurrentDictionary<Type, SqlPropertyMetadata[]> _metaDataPropertyCache = new ConcurrentDictionary<Type, SqlPropertyMetadata[]>();

    public static List<PropertyInfo> FindClassProperties<TEntity>(this Type objectType, List<PropertyInfo> allProperties)
    {
        if (_reflectionPropertyCache.TryGetValue(objectType, out var cachedEntry))
        {
            allProperties.AddRange(cachedEntry.Where(c => !allProperties.Any(a => a.Name == c.Name)));
            return allProperties;
        }

        allProperties.AddRange(objectType.GetProperties()
            .OrderByDescending(x => x.GetCustomAttribute<IdentityAttribute>() != null)
            .ThenByDescending(x => x.GetCustomAttribute<KeyAttribute>() != null)
            .ThenBy(p => p.GetCustomAttributes<ColumnAttribute>()
                .Select(a => a.Order)
                .DefaultIfEmpty(int.MaxValue)
                .FirstOrDefault()).Where(o => !allProperties.Any(a => a.Name == o.Name)));

        _reflectionPropertyCache.TryAdd(objectType, allProperties.ToArray());
        foreach (var pi in allProperties)
        {
            var type = pi.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                type = type.GetGenericArguments()[0];
                return type.FindClassProperties<TEntity>(allProperties);
            }
        }
        return allProperties;
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
