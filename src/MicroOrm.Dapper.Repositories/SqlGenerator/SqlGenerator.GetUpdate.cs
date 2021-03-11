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

            var query = new SqlQuery();

            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName)
                .Append(" ");

            if (includes?.Length > 0)
            {
                var joinsBuilder = AppendJoinToUpdate(entity, query, includes);
                query.SqlBuilder.Append("SET ");
                query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties, UseQuotationMarks == true));
                query.SqlBuilder.Append(joinsBuilder);
            }
            else
            {
                query.SqlBuilder.Append("SET ");
                query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties, UseQuotationMarks == true));
            }

            query.SqlBuilder.Append(" WHERE ");

            query.SqlBuilder.Append(string.Join(" AND ", KeySqlProperties.Where(p => !p.IgnoreUpdate)
                .Select(p => $"{TableName}.{p.ColumnName} = @{entity.GetType().Name}{p.PropertyName}")));

            if (query.Param == null || !(query.Param is Dictionary<string, object> parameters))
                parameters = new Dictionary<string, object>();

            foreach (var metadata in properties.Concat(KeySqlProperties))
                parameters.Add($"{entity.GetType().Name}{metadata.PropertyName}", entity.GetType().GetProperty(metadata.PropertyName).GetValue(entity, null));

            query.SetParam(parameters);

            return query;
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

            var query = new SqlQuery();

            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName)
                .Append(" ");

            if (includes?.Length > 0)
            {
                var joinsBuilder = AppendJoinToUpdate(entity, query, includes);
                query.SqlBuilder.Append("SET ");
                query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties, UseQuotationMarks == true));
                query.SqlBuilder.Append(joinsBuilder);
            }
            else
            {
                query.SqlBuilder.Append("SET ");
                query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties, UseQuotationMarks == true));
            }

            query.SqlBuilder
                .Append(" ");

            AppendWherePredicateQuery(query, predicate, QueryType.Update);

            var parameters = (Dictionary<string, object>)query.Param;
            foreach (var metadata in properties)
                parameters.Add($"{entity.GetType().Name}{metadata.PropertyName}", entity.GetType().GetProperty(metadata.PropertyName).GetValue(entity, null));

            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, object setPropertyObj)
        {
            var setProperties = setPropertyObj.GetType().GetProperties();
            var properties = SqlProperties
                .Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate
                 && setProperties.Any(k => k.Name.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            var query = new SqlQuery();
            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName)
                .Append(" ");
            query.SqlBuilder.Append("SET ");
            query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties, UseQuotationMarks == true));
            query.SqlBuilder.Append(" ");
            AppendWherePredicateQuery(query, predicate, QueryType.Update);

            var parameters = (Dictionary<string, object>)query.Param;
            foreach (var metadata in properties)
            {
                var setProp = setProperties.FirstOrDefault(p => p.Name.Equals(metadata.PropertyName, StringComparison.OrdinalIgnoreCase));
                if (setProp == null) continue;
                parameters.Add($"{typeof(TEntity).Name}{metadata.PropertyName}", setProp.GetValue(setPropertyObj));
            }
            return query;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetUpdate(Expression<Func<TEntity, bool>> predicate, Dictionary<string, object> setPropertyDict)
        {
            var propNameExceptItems = setPropertyDict.Keys.Except(SqlProperties.Select(p => p.PropertyName));
            if (propNameExceptItems.Any())
            {
                string keys = string.Join(",", propNameExceptItems.Select(p => p));
                throw new ArgumentException(string.Concat(nameof(setPropertyDict), "content error detail:", $" [{keys}] not equal entity column name"));
            }
            var properties = SqlProperties
                .Where(p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)) && !p.IgnoreUpdate
                 && setPropertyDict.Any(k => k.Key.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            var query = new SqlQuery();
            query.SqlBuilder
                .Append("UPDATE ")
                .Append(TableName)
                .Append(" ");
            query.SqlBuilder.Append("SET ");
            query.SqlBuilder.Append(GetFieldsUpdate(TableName, properties, UseQuotationMarks == true));
            query.SqlBuilder.Append(" ");
            AppendWherePredicateQuery(query, predicate, QueryType.Update);

            var parameters = (Dictionary<string, object>)query.Param;
            foreach (var metadata in properties)
            {
                var value = setPropertyDict.FirstOrDefault(p => p.Key.Equals(metadata.PropertyName, StringComparison.OrdinalIgnoreCase)).Value;
                parameters.Add($"{typeof(TEntity).Name}{metadata.PropertyName}", value);
            }
            return query;
        }

        private string GetFieldsUpdate(string tableName, IEnumerable<SqlPropertyMetadata> properties, bool useMarks)
        {
            if (Provider == SqlProvider.SQLite)
            {
                //***
                //*** Building update query for sqlite
                //***
                return string.Join(", ", properties
                    .Select(p => $"{(useMarks ? p.ColumnName : p.CleanColumnName)} = @{p.PropertyInfo.ReflectedType.Name}{p.PropertyName}"));
            }
            else
            {
                return string.Join(", ", properties
                    .Select(p => $"{tableName}.{(useMarks ? p.ColumnName : p.CleanColumnName)} = @{p.PropertyInfo.ReflectedType.Name}{p.PropertyName}"));
            }
        }
    }
}
