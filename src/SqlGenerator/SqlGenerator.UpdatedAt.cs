using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;

public partial class SqlGenerator<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Write the current time to the [UpdatedAt] property of the entity
    /// </summary>
    private void SetUpdatedAt(object entity)
    {
        if (HasUpdatedAt && UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>() is { } attribute)
            UpdatedAtProperty.SetValue(entity, GetUpdatedAtValue(attribute));
    }

    /// <summary>
    /// Get the [UpdatedAt] column and its value when it isn't among the columns already being set, otherwise null
    /// </summary>
    private SqlPropertyMetadata? GetUpdatedAtToSet(IEnumerable<SqlPropertyMetadata> properties, out DateTime value)
    {
        value = default;

        if (!HasUpdatedAt || UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>() is not { } attribute)
            return null;

        // The metadata of SqlProperties is the configured one, UpdatedAtPropertyMetadata keeps the raw column name
        var metadata = SqlProperties.FirstOrDefault(p => p.PropertyName.Equals(UpdatedAtProperty.Name, StringComparison.OrdinalIgnoreCase));
        if (metadata == null || metadata.IgnoreUpdate)
            return null;

        if (properties.Any(p => p.PropertyName.Equals(metadata.PropertyName, StringComparison.OrdinalIgnoreCase)))
            return null;

        value = GetUpdatedAtValue(attribute);
        return metadata;
    }

    private static DateTime GetUpdatedAtValue(UpdatedAtAttribute attribute)
    {
        var offset = attribute.TimeKind == DateTimeKind.Local
            ? new DateTimeOffset(DateTime.Now)
            : new DateTimeOffset(DateTime.UtcNow);

        if (attribute.OffSet != 0)
            offset = offset.ToOffset(TimeSpan.FromHours(attribute.OffSet));

        return offset.DateTime;
    }
}
