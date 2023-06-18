using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories.SqlGenerator;


public partial class SqlGenerator<TEntity>
    where TEntity : class
{
    private SqlQuery GetSelect(Expression<Func<TEntity, bool>>? predicate, bool firstOnly,
        FilterData? filterData,
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
            if (Provider == SqlProvider.Oracle)
                sqlQuery.SqlBuilder.Append("FETCH FIRST 1 ROW ONLY");
            else if (Provider != SqlProvider.MSSQL)
                sqlQuery.SqlBuilder
                    .Append("LIMIT 1");
        }

        return sqlQuery;
    }

    private void SetLimit(SqlQuery sqlQuery, FilterData? filterData)
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
        }
        else if (Provider == SqlProvider.Oracle)
        {
            sqlQuery.SqlBuilder.Append("OFFSET ");
            sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Offset ?? 0);
            sqlQuery.SqlBuilder.Append(" ROWS FETCH NEXT ");
            sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Limit);
            sqlQuery.SqlBuilder.Append(" ROWS ONLY");
        }
        else
        {
            sqlQuery.SqlBuilder.Append("LIMIT ");
            sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Limit);
            if (filterData.LimitInfo.Offset != null)
            {
                sqlQuery.SqlBuilder.Append(" OFFSET ");
                sqlQuery.SqlBuilder.Append(filterData.LimitInfo.Offset);
            }
        }

        if (!filterData.LimitInfo.Permanent)
            filterData.LimitInfo = null;
    }

    /// <summary>
    /// Set order by in query; DapperRepository.SetOrderBy must be called first.
    /// </summary>
    private void SetOrder(SqlQuery sqlQuery, FilterData? filterData)
    {
        if (filterData?.OrderInfo == null)
            return;

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

        if (filterData.OrderInfo.Columns == null)
            return;

        var i = 0;
        var count = filterData.OrderInfo.Columns.Count;
        foreach (var col in filterData.OrderInfo.Columns)
        {
            if (UseQuotationMarks == true && Provider != SqlProvider.SQLite && Provider != SqlProvider.Oracle)
            {
                sqlQuery.SqlBuilder.Append(AddProviderQuotationMarks(col));
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
            filterData.OrderInfo.Columns?.Clear();
            filterData.OrderInfo.Columns = null;
            filterData.OrderInfo = null;
        }

        filterData.Ordered = true;
    }

    /// <summary>
    /// Adds quotation marks to column identifier based on the provider
    /// </summary>
    /// <param name="columnIdentifier"></param>
    /// <returns>String with quotation marks added</returns>
    private string AddProviderQuotationMarks(string columnIdentifier)
    {
        // If a table identifier is passed in with field the data string will be table.field instead of just rapping quotations around the
        // string [table.field] or `table.field` we have to replace the seperator . with ].[ or `.` to get [table].[field] or `table`.`field`
        switch (Provider)
        {
            case SqlProvider.MSSQL:
                return $"[{columnIdentifier.Replace(".", "].[")}]";

            case SqlProvider.MySQL:
            case SqlProvider.SQLite:
                return $"`{columnIdentifier.Replace(".", "`.`")}`";

            case SqlProvider.PostgreSQL:
                return $"\"{columnIdentifier.Replace(".", "\".\"")}\"";

            default:
                return columnIdentifier;
        }
    }

    /// <summary>
    /// Set group by in query; DapperRepository.GroupBy must be called first.
    /// </summary>
    private void GroupBy(SqlQuery sqlQuery, FilterData? filterData)
    {
        if (filterData?.GroupInfo?.Columns == null)
            return;

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


    public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>>? predicate, FilterData? filterData, params Expression<Func<TEntity, object>>[] includes)
    {
        return GetSelect(predicate, includes.Length == 0, filterData, includes);
    }


    public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>>? predicate, FilterData? filterData, params Expression<Func<TEntity, object>>[] includes)
    {
        return GetSelect(predicate, false, filterData, includes);
    }


    public virtual SqlQuery GetSelectById(object id, FilterData? filterData, params Expression<Func<TEntity, object>>[] includes)
    {
        var param = GetKeysParam(id);

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

        for (var index = 0; index < KeySqlProperties.Length; index++)
            sqlQuery.SqlBuilder
                .Append(index == 0 ? "WHERE " : "AND ")
                .Append(TableName)
                .Append(".")
                .Append(KeySqlProperties[index].ColumnName)
                .Append(" = ")
                .Append(ParameterSymbol)
                .Append(KeySqlProperties[index].PropertyName)
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
        {
            if (Provider == SqlProvider.Oracle)
                sqlQuery.SqlBuilder.Append("FETCH FIRST 1 ROWS ONLY");
            else
                sqlQuery.SqlBuilder.Append("LIMIT 1");
        }

        sqlQuery.SetParam(param);
        return sqlQuery;
    }


    public virtual SqlQuery GetSelectBetween(object from, object to, FilterData? filterData, Expression<Func<TEntity, object>> btwField)
    {
        return GetSelectBetween(from, to, filterData, btwField, null);
    }


    public virtual SqlQuery GetSelectBetween(object from, object to, FilterData? filterData, Expression<Func<TEntity, object>> btwField,
        Expression<Func<TEntity, bool>>? predicate)
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

    internal object GetKeysParam(object id)
    {
        if (KeySqlProperties.Length < 2)
            return new Dictionary<string, object> { { KeySqlProperties[0].PropertyName, id } };

        if (id is not Array array)
            return id;

        if (array.Length != KeySqlProperties.Length)
            throw new ArgumentException("GetSelectById id(Array) length not equals key properties count", nameof(id));

        var dictionary = new Dictionary<string, object>();

        for (var index = 0; index < KeySqlProperties.Length; index++)
        {
            if (array.GetValue(index) is not { } value)
                throw new ArgumentException($"Key value is null in {index}", nameof(id));

            dictionary[KeySqlProperties[index].PropertyName] = value;
        }

        return dictionary;
    }

    private SqlQuery InitBuilderSelect(bool firstOnly, FilterData? filterData)
    {
        var query = new SqlQuery();
        query.SqlBuilder.Append("SELECT ");

        if (Provider == SqlProvider.MSSQL)
        {
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
        }

        query.SqlBuilder.Append(filterData?.SelectInfo?.Columns == null
            ? GetFieldsSelect(TableName, SqlProperties, UseQuotationMarks == true)
            : GetFieldsSelect(filterData.SelectInfo.Columns));
        return query;
    }

    private static string GetFieldsSelect(IEnumerable<string> properties)
    {
        return string.Join(", ", properties);
    }

    private static string GetFieldsSelect(string? tableName, IEnumerable<SqlPropertyMetadata> properties, bool useQuotation)
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
