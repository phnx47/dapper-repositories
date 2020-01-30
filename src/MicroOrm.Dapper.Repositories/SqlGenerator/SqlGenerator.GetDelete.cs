using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Attributes;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual SqlQuery GetDelete(TEntity entity)
        {
            var sqlQuery = new SqlQuery();
            var whereAndSql = 
                string.Join(" AND ", KeySqlProperties.Select(p => string.Format("{0}.{1} = @{2}", TableName, p.ColumnName, p.PropertyName)));

            if (!LogicalDelete)
            {
                sqlQuery.SqlBuilder
                    .Append("DELETE FROM ")
                    .Append(TableName)
                    .Append(" WHERE ")
                    .Append(whereAndSql);
            }
            else
            {                
                sqlQuery.SqlBuilder
                    .Append("UPDATE ")
                    .Append(TableName)
                    .Append(" SET ")
                    .Append(StatusPropertyName)
                    .Append(" = ")
                    .Append(LogicalDeleteValue);

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

                    sqlQuery.SqlBuilder
                        .Append(", ")
                        .Append(UpdatedAtPropertyMetadata.ColumnName)
                        .Append(" = @")
                        .Append(UpdatedAtPropertyMetadata.PropertyName);
                }

                sqlQuery.SqlBuilder 
                    .Append(" WHERE ")
                    .Append(whereAndSql);
            }

            sqlQuery.SetParam(entity);
            return sqlQuery;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetDelete(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlQuery = new SqlQuery();

            if (!LogicalDelete)
            {
                sqlQuery.SqlBuilder
                    .Append("DELETE FROM ")
                    .Append(TableName);
            }
            else
            {
                sqlQuery.SqlBuilder
                    .Append("UPDATE ")
                    .Append(TableName)
                    .Append(" SET ")
                    .Append(StatusPropertyName)
                    .Append(" = ")
                    .Append(LogicalDeleteValue);

                if (HasUpdatedAt)
                    sqlQuery.SqlBuilder
                        .Append(", ")
                        .Append(UpdatedAtPropertyMetadata.ColumnName)
                        .Append(" = @")
                        .Append(UpdatedAtPropertyMetadata.PropertyName);

               
            }
            sqlQuery.SqlBuilder.Append(" ");
            AppendWherePredicateQuery(sqlQuery, predicate, QueryType.Delete);
            return sqlQuery;
        }
    }
}
