using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Extensions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(TEntity entity)
        {
            var properties = SqlProperties.Where(p =>
                !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();
            if (!properties.Any())
                throw new ArgumentException("Can't update without [Key]");

            if (HasUpdatedAt)
            {
                var attribute = UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>();
                UpdatedAtProperty.SetValue(entity, TimeZoneInfo.ConvertTime(DateTime.Now, attribute.TimeZone));
            }

            var query = new SqlQuery(entity);

            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName)
                .Append(" SET ");

            query.SqlBuilder.Append(string.Join(", ", properties
                .Select(p => $"{p.ColumnName} = @{p.PropertyName}")));

            query.SqlBuilder.Append(" WHERE ");

            query.SqlBuilder.Append(string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate)
                .Select(p => $"{p.ColumnName} = @{p.PropertyName}")));

            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, TEntity entity)
        {
            var properties = SqlProperties.Where(p =>
                !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();

            if (HasUpdatedAt)
            {
                var attribute = UpdatedAtProperty.GetCustomAttribute<UpdatedAtAttribute>();
                UpdatedAtProperty.SetValue(entity, TimeZoneInfo.ConvertTime(DateTime.Now, attribute.TimeZone));
            }

            var query = new SqlQuery(entity);

            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName)
                .Append(" SET ");

            query.SqlBuilder.Append(string.Join(", ", properties
                .Select(p => $"{p.ColumnName} = @{p.PropertyName}")));

            query.SqlBuilder
                .Append(" ");
            
            AppendWherePredicateQuery(query, predicate, QueryType.Update);

            var entityType = entity.GetType();
            var parameters = properties.ToDictionary(property => property.PropertyName, property => entityType.GetProperty(property.PropertyName)?.GetValue(entity, null));

            if (query.Param is Dictionary<string, object> whereParam)
                parameters.AddRange(whereParam);

            query.SetParam(parameters);

            return query;
        }
    }
}
