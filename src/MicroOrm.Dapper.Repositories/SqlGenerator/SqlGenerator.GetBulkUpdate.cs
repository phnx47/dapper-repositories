using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual SqlQuery GetBulkUpdate(IEnumerable<TEntity> entities)
        {
            var entitiesArray = entities as TEntity[] ?? entities.ToArray();
            if (!entitiesArray.Any())
                throw new ArgumentException("collection is empty");

            var entityType = entitiesArray[0].GetType();

            var properties = SqlProperties.Where(p =>
                !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();

            var query = new SqlQuery();

            var parameters = new Dictionary<string, object>();

            for (var i = 0; i < entitiesArray.Length; i++)
            {
                var entity = entitiesArray[i];
                if (HasUpdatedAt)
                {
                    var attribute = UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>();
                    var offset = attribute.TimeKind == DateTimeKind.Local
                        ? new DateTimeOffset(DateTime.Now)
                        : new DateTimeOffset(DateTime.UtcNow);
                    if (attribute.OffSet != 0)
                    {
                        offset = offset.ToOffset(TimeSpan.FromHours(attribute.OffSet));
                    }

                    UpdatedAtProperty.SetValue(entity, offset.DateTime);
                }

                if (i > 0)
                    query.SqlBuilder.Append("; ");

                query.SqlBuilder.Append(
                    $"UPDATE {TableName} SET {string.Join(", ", properties.Select(p => $"{p.ColumnName} = @{p.PropertyName}{i}"))} WHERE {string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate).Select(p => $"{p.ColumnName} = @{p.PropertyName}{i}"))}");

                // ReSharper disable PossibleNullReferenceException
                foreach (var property in properties)
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entity, null));

                foreach (var property in KeySqlProperties.Where(p => !p.IgnoreUpdate))
                    parameters.Add(property.PropertyName + i, entityType.GetProperty(property.PropertyName).GetValue(entity, null));

                // ReSharper restore PossibleNullReferenceException
            }

            query.SetParam(parameters);

            return query;
        }
    }
}
