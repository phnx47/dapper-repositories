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
        public virtual SqlQuery GetUpdate(TEntity entity, params Expression<Func<TEntity, object>>[] includes)
        {
            var properties = SqlProperties.Where(p =>
                !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();
            
            if (!properties.Any())
                throw new ArgumentException("Can't update without [Key]");

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


            var parameters = properties.Concat(KeySqlProperties).ToDictionary(prop => $"{prop.PropertyInfo.DeclaringType.Name}{prop.PropertyName}", prop => entity.GetType().GetProperty(prop.PropertyName).GetValue(entity, null));

            var query = new SqlQuery(parameters);

            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName)
                .Append(" ");

            if (includes?.Length > 0)
            {
                var joinsBuilder = AppendJoinToUpdate(entity, query, includes);
                query.SqlBuilder.Append("SET ");
                query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties));
                query.SqlBuilder.Append(joinsBuilder);
            }
            else
            {
                query.SqlBuilder.Append(" SET ");
                query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties));
            }

            query.SqlBuilder.Append(" WHERE ");

            query.SqlBuilder.Append(string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate)
                .Select(p => $"{TableName}.{p.ColumnName} = @{p.PropertyInfo.DeclaringType.Name}{p.PropertyName}")));

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
                var offset = attribute.TimeKind == DateTimeKind.Local
                    ? new DateTimeOffset(DateTime.Now)
                    : new DateTimeOffset(DateTime.UtcNow);
                if (attribute.OffSet != 0)
                {
                    offset = offset.ToOffset(TimeSpan.FromHours(attribute.OffSet));
                }

                UpdatedAtProperty.SetValue(entity, offset.DateTime);
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

        private static string GetFieldsUpdate(string tableName, IEnumerable<SqlPropertyMetadata> properties)
        {
            return string.Join(", ", properties
                .Select(p => $"{tableName}.{p.ColumnName} = @{p.PropertyInfo.DeclaringType.Name}{p.PropertyName}"));
        }

        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, TEntity entity, params Expression<Func<TEntity, object>>[] includes)
        {
            var properties = SqlProperties.Where(p =>
                !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate).ToArray();

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

            var query = new SqlQuery(entity);

            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName);

            var joinsBuilder = AppendJoinToUpdate(entity, query, includes);
            query.SqlBuilder.Append(" SET ");
            query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties));
            query.SqlBuilder.Append(joinsBuilder);

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
