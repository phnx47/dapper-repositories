using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            GroupBy(sqlQuery, filterData);

            if (!firstOnly)
            {
                SetLimit(sqlQuery, filterData);
            }
            else
            {
                if (Provider != SqlProvider.MSSQL)
                    sqlQuery.SqlBuilder
                        .Append("LIMIT 1");
            }

            return sqlQuery;
        }

        private void SetLimit(SqlQuery sqlQuery, FilterData filterData)
        {
            if (filterData?.LimitInfo == null)
                return;

            if (Provider == SqlProvider.MSSQL)
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
        private void SetOrder(SqlQuery sqlQuery, FilterData filterData)
        {
            if (filterData?.OrderInfo == null) return;

            sqlQuery.SqlBuilder.Append("ORDER BY ");
            if (!string.IsNullOrEmpty(filterData.OrderInfo.CustomQuery))
            {
                sqlQuery.SqlBuilder.Append(filterData.OrderInfo.CustomQuery);
                if (!filterData.OrderInfo.Permanent)
                {
                    filterData.OrderInfo.CustomQuery = null;
                }

                filterData.Ordered = true;
                return;
            }

            var i = 0;
            var count = filterData.OrderInfo.Columns.Count;
            foreach (var col in filterData.OrderInfo.Columns)
            {
                if (UseQuotationMarks == true && Provider != SqlProvider.SQLite)
                {
                    sqlQuery.SqlBuilder.Append(Provider == SqlProvider.MSSQL ? $"[{col}]" : $"`{col}`");
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
                i++;
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

        /// <summary>
        /// Set group by in query; DapperRepository.GroupBy must be called first. 
        /// </summary>
        private void GroupBy(SqlQuery sqlQuery, FilterData filterData)
        {
            if (filterData?.GroupInfo == null) return;

            sqlQuery.SqlBuilder.Append("GROUP BY ");
            if (!string.IsNullOrEmpty(filterData.GroupInfo.CustomQuery))
            {
                sqlQuery.SqlBuilder.Append(filterData.GroupInfo.CustomQuery);
                if (!filterData.GroupInfo.Permanent)
                {
                    filterData.GroupInfo.CustomQuery = null;
                }

                filterData.Ordered = true;
                return;
            }

            var i = 0;
            var count = filterData.GroupInfo.Columns.Count;
            foreach (var col in filterData.GroupInfo.Columns)
            {
                if (UseQuotationMarks == true && Provider != SqlProvider.SQLite)
                {
                    sqlQuery.SqlBuilder.Append(Provider == SqlProvider.MSSQL ? $"[{col}]" : $"`{col}`");
                }
                else
                {
                    sqlQuery.SqlBuilder.Append(col);
                }

                if (i >= count - 1)
                {
                    break;
                }

                sqlQuery.SqlBuilder.Append(",");
                i++;
            }

            sqlQuery.SqlBuilder.Append(" ");

            if (!filterData.GroupInfo.Permanent)
            {
                filterData.GroupInfo.Columns.Clear();
                filterData.GroupInfo.Columns = null;
                filterData.GroupInfo = null;
            }

            filterData.Ordered = true;
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate, FilterData filterData, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, filterData, includes);
        }

        /// <inheritdoc />
        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate, FilterData filterData, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, filterData, includes);
        }

        /// <inheritdoc />
        public SqlQuery GetSelectById(object id, FilterData filterData, params Expression<Func<TEntity, object>>[] includes)
        {
            if (KeySqlProperties.Length != 1)
                throw new NotSupportedException("GetSelectById support only 1 key");

            var keyProperty = KeySqlProperties[0];

            var sqlQuery = InitBuilderSelect(includes.Length == 0, filterData);

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

            if (includes.Length == 0 && Provider != SqlProvider.MSSQL)
                sqlQuery.SqlBuilder
                    .Append("LIMIT 1");

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

            if (Provider == SqlProvider.MSSQL)
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

            query.SqlBuilder.Append(filterData?.SelectInfo?.Columns == null
                ? GetFieldsSelect(TableName, SqlProperties, UseQuotationMarks == true)
                : GetFieldsSelect(filterData.SelectInfo.Columns));

            return query;
        }

        private static string GetFieldsSelect(List<string> properties)
        {
            return string.Join(", ", properties);
        }

        private static string GetFieldsSelect(string tableName, IEnumerable<SqlPropertyMetadata> properties, bool useQuotation)
        {
            //Projection function
            string ProjectionFunction(SqlPropertyMetadata p)
            {
                return !string.IsNullOrEmpty(p.Alias)
                    ? $"{tableName}.{(useQuotation ? p.ColumnName : p.CleanColumnName)} AS {p.PropertyName}"
                    : $"{tableName}.{(useQuotation ? p.ColumnName : p.CleanColumnName)}";
            }

            return string.Join(", ", properties.Select(ProjectionFunction));
        }
    }
}
