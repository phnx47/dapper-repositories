using System;
using System.Linq;
using System.Linq.Expressions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual SqlQuery GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlQuery = new SqlQuery();

            sqlQuery.SqlBuilder
                .Append("SELECT COUNT(*) FROM ")
                .Append(TableName)
                .Append(" ");

            AppendWherePredicateQuery(sqlQuery, predicate, QueryType.Select);

            return sqlQuery;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetCount(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField)
        {
            var propertyName = ExpressionHelper.GetPropertyName(distinctField);
            var property = SqlProperties.First(x => x.PropertyName == propertyName);
            var sqlQuery = InitBuilderCountWithDistinct(property);

            sqlQuery.SqlBuilder
                .Append(" FROM ")
                .Append(TableName)
                .Append(" ");

            AppendWherePredicateQuery(sqlQuery, predicate, QueryType.Select);

            return sqlQuery;
        }

        private SqlQuery InitBuilderCountWithDistinct(SqlPropertyMetadata sqlProperty)
        {
            var query = new SqlQuery();
            query.SqlBuilder.Append("SELECT COUNT(DISTINCT ");

            query.SqlBuilder
                .Append(TableName)
                .Append(".")
                .Append(sqlProperty.ColumnName)
                .Append(")");

            if (sqlProperty.Alias != null)
                query.SqlBuilder
                    .Append(" AS ")
                    .Append(sqlProperty.PropertyName);

            return query;
        }
    }
}
