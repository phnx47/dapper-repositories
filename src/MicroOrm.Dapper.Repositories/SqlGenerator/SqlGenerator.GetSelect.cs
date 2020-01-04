using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.Config;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        private SqlQuery GetSelect(Expression<Func<TEntity, bool>> predicate, bool firstOnly,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = InitBuilderSelect(firstOnly);

            if (includes.Length > 0)
            {
                var joinsBuilder = AppendJoinToSelect(sqlQuery, includes);
                sqlQuery.SqlBuilder
                    .Append(" FROM ")
                    .Append(TableName)
                    .Append(" ");

                sqlQuery.SqlBuilder.Append(joinsBuilder);
            }
            else
            {
                sqlQuery.SqlBuilder
                    .Append(" FROM ")
                    .Append(TableName)
                    .Append(" ");
            }

            AppendWherePredicateQuery(sqlQuery, predicate, QueryType.Select);

            SetOrder(TableName, sqlQuery);

            if (firstOnly)
            {
                if (MicroOrmConfig.SqlProvider != SqlProvider.MSSQL)
                    sqlQuery.SqlBuilder.Append("LIMIT 1");
            }
            else
                SetLimit(sqlQuery);

            return sqlQuery;
        }

        private void SetLimit(SqlQuery sqlQuery)
        {
            if (FilterData.LimitInfo == null)
                return;

            if (MicroOrmConfig.SqlProvider == SqlProvider.MSSQL)
            {
                if (!FilterData.Ordered)
                    return;

                sqlQuery.SqlBuilder.Append("OFFSET ");
                sqlQuery.SqlBuilder.Append(FilterData.LimitInfo.Offset ?? 0);
                sqlQuery.SqlBuilder.Append(" ROWS FETCH NEXT ");
                sqlQuery.SqlBuilder.Append(FilterData.LimitInfo.Limit);
                sqlQuery.SqlBuilder.Append(" ROWS ONLY");
                return;
            }

            sqlQuery.SqlBuilder.Append("LIMIT ");
            sqlQuery.SqlBuilder.Append(FilterData.LimitInfo.Limit);
            if (FilterData.LimitInfo.Offset != null)
            {
                sqlQuery.SqlBuilder.Append(" OFFSET ");
                sqlQuery.SqlBuilder.Append(FilterData.LimitInfo.Offset);
            }

            if (!FilterData.LimitInfo.Permanent)
                FilterData.LimitInfo = null;
        }

        /// <summary>
        /// Set order by in query; DapperRepository.SetOrderBy must be called first. 
        /// </summary>
        private void SetOrder(string tableName, SqlQuery sqlQuery)
        {
            if (FilterData.OrderInfo == null) return;

            sqlQuery.SqlBuilder.Append("ORDER BY ");

            var count = FilterData.OrderInfo.Columns.Count;
            for (var i = 0; i < count; i++)
            {
                var col = FilterData.OrderInfo.Columns[i];
                sqlQuery.SqlBuilder.Append(tableName);
                sqlQuery.SqlBuilder.Append(".");
                if (MicroOrmConfig.UseQuotationMarks && MicroOrmConfig.SqlProvider != SqlProvider.SQLite)
                {
                    sqlQuery.SqlBuilder.Append(MicroOrmConfig.SqlProvider == SqlProvider.MSSQL ? $"[{col}]" : $"`{col}`");
                }
                else
                {
                    sqlQuery.SqlBuilder.Append(col);
                }

                if (i >= count - 1)
                {
                    sqlQuery.SqlBuilder.Append(" ");
                    sqlQuery.SqlBuilder.Append(FilterData.OrderInfo.Direction);
                    break;
                }

                sqlQuery.SqlBuilder.Append(",");
            }

            sqlQuery.SqlBuilder.Append(" ");

            if (!FilterData.OrderInfo.Permanent)
                FilterData.OrderInfo = null;

            FilterData.Ordered = true;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, includes);
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, includes);
        }

        /// <inheritdoc />
        public SqlQuery GetSelectById(object id, params Expression<Func<TEntity, object>>[] includes)
        {
            if (KeySqlProperties.Length != 1)
                throw new NotSupportedException("GetSelectById support only 1 key");

            var keyProperty = KeySqlProperties[0];

            var sqlQuery = InitBuilderSelect(true);

            if (includes.Length > 0)
            {
                var joinsBuilder = AppendJoinToSelect(sqlQuery, includes);
                sqlQuery.SqlBuilder
                    .Append(" FROM ")
                    .Append(TableName)
                    .Append(" ");

                sqlQuery.SqlBuilder.Append(joinsBuilder);
            }
            else
            {
                sqlQuery.SqlBuilder
                    .Append(" FROM ")
                    .Append(TableName)
                    .Append(" ");
            }

            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                {keyProperty.PropertyName, id}
            };

            sqlQuery.SqlBuilder
                .Append("WHERE ")
                .Append(TableName)
                .Append(".")
                .Append(keyProperty.ColumnName)
                .Append(" = @")
                .Append(keyProperty.PropertyName)
                .Append(" ");

            if (LogicalDelete)
                sqlQuery.SqlBuilder
                    .Append("AND ")
                    .Append(TableName)
                    .Append(".")
                    .Append(StatusPropertyName)
                    .Append(" != ")
                    .Append(LogicalDeleteValue)
                    .Append(" ");

            if (MicroOrmConfig.SqlProvider == SqlProvider.MySQL || MicroOrmConfig.SqlProvider == SqlProvider.PostgreSQL)
                sqlQuery.SqlBuilder.Append("LIMIT 1");

            sqlQuery.SetParam(dictionary);
            return sqlQuery;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwField)
        {
            return GetSelectBetween(from, to, btwField, null);
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> predicate)
        {
            var fieldName = ExpressionHelper.GetPropertyName(btwField);
            var columnName = SqlProperties.First(x => x.PropertyName == fieldName).ColumnName;
            var query = GetSelectAll(predicate);

            query.SqlBuilder
                .Append(predicate == null && !LogicalDelete ? "WHERE" : "AND")
                .Append(" ")
                .Append(TableName)
                .Append(".")
                .Append(columnName)
                .Append(" BETWEEN '")
                .Append(from)
                .Append("' AND '")
                .Append(to)
                .Append("'");

            return query;
        }

        private SqlQuery InitBuilderSelect(bool firstOnly)
        {
            var query = new SqlQuery();
            query.SqlBuilder.Append("SELECT ");

            if (MicroOrmConfig.SqlProvider == SqlProvider.MSSQL)
                if (firstOnly)
                    query.SqlBuilder.Append("TOP 1 ");
                else
                {
                    if (FilterData.LimitInfo != null && FilterData.OrderInfo == null)
                    {
                        query.SqlBuilder.Append("TOP (");
                        query.SqlBuilder.Append(FilterData.LimitInfo.Limit);
                        query.SqlBuilder.Append(") ");
                    }
                }

            query.SqlBuilder.Append(GetFieldsSelect(TableName, SqlProperties));

            return query;
        }

        private static string GetFieldsSelect(string tableName, IEnumerable<SqlPropertyMetadata> properties)
        {
            //Projection function
            string ProjectionFunction(SqlPropertyMetadata p)
            {
                return !string.IsNullOrEmpty(p.Alias)
                    ? $"{tableName}.{p.ColumnName} AS {p.PropertyName}"
                    : $"{tableName}.{p.ColumnName}";
            }

            return string.Join(", ", properties.Select(ProjectionFunction));
        }
    }
}
