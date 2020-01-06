using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        private SqlQuery GetSelect(Expression<Func<TEntity, bool>> predicate, bool firstOnly,
            FilterData filterData,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = InitBuilderSelect(firstOnly, filterData);

            if (includes.Length > 0)
            {
                var joinsBuilder = AppendJoinToSelect(sqlQuery, filterData?.SelectInfo != null, includes);
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

            if (filterData?.SelectInfo != null)
            {
                if (!filterData.SelectInfo.Permanent)
                {
                    filterData.SelectInfo.Columns.Clear();
                    filterData.SelectInfo.Columns = null;
                    filterData.SelectInfo = null;
                }
            }

            AppendWherePredicateQuery(sqlQuery, predicate, QueryType.Select);

            SetOrder(sqlQuery, filterData);

            if (firstOnly)
            {
                if (MicroOrmConfig.SqlProvider != SqlProvider.MSSQL)
                    sqlQuery.SqlBuilder.Append("LIMIT 1");
            }
            else
                SetLimit(sqlQuery, filterData);

            return sqlQuery;
        }

        private static void SetLimit(SqlQuery sqlQuery, FilterData filterData)
        {
            if (filterData?.LimitInfo == null)
                return;

            if (MicroOrmConfig.SqlProvider == SqlProvider.MSSQL)
            {
                if (!filterData.Ordered)
                    return;

                sqlQuery.SqlBuilder.Append("OFFSET ");
                sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Offset ?? 0);
                sqlQuery.SqlBuilder.Append(" ROWS FETCH NEXT ");
                sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Limit);
                sqlQuery.SqlBuilder.Append(" ROWS ONLY");
                return;
            }

            sqlQuery.SqlBuilder.Append("LIMIT ");
            sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Limit);
            if (filterData.LimitInfo.Offset != null)
            {
                sqlQuery.SqlBuilder.Append(" OFFSET ");
                sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Offset);
            }

            if (!filterData.LimitInfo.Permanent)
                filterData.LimitInfo = null;
        }

        /// <summary>
        /// Set order by in query; DapperRepository.SetOrderBy must be called first. 
        /// </summary>
        private static void SetOrder(SqlQuery sqlQuery, FilterData filterData)
        {
            if (filterData?.OrderInfo == null) return;

            sqlQuery.SqlBuilder.Append("ORDER BY ");

            var count = filterData.OrderInfo.Columns.Count;
            for (var i = 0; i < count; i++)
            {
                var col = filterData.OrderInfo.Columns[i];

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
                    sqlQuery.SqlBuilder.Append(filterData.OrderInfo.Direction);
                    break;
                }

                sqlQuery.SqlBuilder.Append(",");
            }

            sqlQuery.SqlBuilder.Append(" ");

            if (!filterData.OrderInfo.Permanent)
            {
                filterData.OrderInfo.Columns.Clear();
                filterData.OrderInfo.Columns = null;
                filterData.OrderInfo = null;
            }

            filterData.Ordered = true;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, null, includes);
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, FilterData filterData, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, filterData, includes);
        }

        /// <inheritdoc />
        public SqlQuery GetSelectById(object id, params Expression<Func<TEntity, object>>[] includes)
        {
            if (KeySqlProperties.Length != 1)
                throw new NotSupportedException("GetSelectById support only 1 key");

            var keyProperty = KeySqlProperties[0];

            var sqlQuery = InitBuilderSelect(true, null);

            if (includes.Length > 0)
            {
                var joinsBuilder = AppendJoinToSelect(sqlQuery, false, includes);
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
        public virtual SqlQuery GetSelectBetween(object from, object to, FilterData filterData, Expression<Func<TEntity, object>> btwField)
        {
            return GetSelectBetween(from, to, filterData, btwField, null);
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectBetween(object from, object to, FilterData filterData, Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate)
        {
            var fieldName = ExpressionHelper.GetPropertyName(btwField);
            var columnName = SqlProperties.First(x => x.PropertyName == fieldName).ColumnName;
            var query = GetSelectAll(predicate, filterData);

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

        private SqlQuery InitBuilderSelect(bool firstOnly, FilterData filterData)
        {
            var query = new SqlQuery();
            query.SqlBuilder.Append("SELECT ");

            if (MicroOrmConfig.SqlProvider == SqlProvider.MSSQL)
                if (firstOnly)
                    query.SqlBuilder.Append("TOP 1 ");
                else
                {
                    if (filterData?.LimitInfo != null && filterData.OrderInfo == null)
                    {
                        query.SqlBuilder.Append("TOP (");
                        query.SqlBuilder.Append(filterData.LimitInfo.Limit);
                        query.SqlBuilder.Append(") ");
                    }
                }

            query.SqlBuilder.Append(filterData?.SelectInfo?.Columns == null ? GetFieldsSelect(TableName, SqlProperties) : GetFieldsSelect(filterData.SelectInfo.Columns));

            return query;
        }

        private static string GetFieldsSelect(List<string> properties)
        {
            return string.Join(", ", properties);
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
