using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MicroOrm.Dapper.Repositories.Extensions;
using MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <inheritdoc />
    public partial class SqlGenerator<TEntity>
        where TEntity : class
    {
        private void AppendWherePredicateQuery(SqlQuery sqlQuery, Expression<Func<TEntity, bool>> predicate, QueryType queryType)
        {
            IDictionary<string, object> dictionaryParams;
            if (sqlQuery.Param is Dictionary<string, object> param)
            {
                dictionaryParams = param;
            }
            else
            {
                dictionaryParams = new Dictionary<string, object>();
            }

            if (predicate != null)
            {
                // WHERE
                var queryProperties = GetQueryProperties(predicate.Body);

                sqlQuery.SqlBuilder.Append("WHERE ");

                var qLevel = 0;
                var sqlBuilder = new StringBuilder();
                var conditions = new List<KeyValuePair<string, object>>();
                BuildQuerySql(queryProperties, ref sqlBuilder, ref conditions, ref qLevel);

                dictionaryParams.AddRange(conditions);

                if (LogicalDelete && queryType == QueryType.Select)
                    sqlQuery.SqlBuilder.AppendFormat("({3}) AND {0}.{1} != {2} ", TableName, StatusPropertyName, LogicalDeleteValue, sqlBuilder);
                else
                    sqlQuery.SqlBuilder.AppendFormat("{0} ", sqlBuilder);
            }
            else
            {
                if (LogicalDelete && queryType == QueryType.Select)
                    sqlQuery.SqlBuilder.AppendFormat("WHERE {0}.{1} != {2} ", TableName, StatusPropertyName, LogicalDeleteValue);
            }

            if (LogicalDelete && HasUpdatedAt && queryType == QueryType.Delete)
                dictionaryParams.Add(UpdatedAtPropertyMetadata.ColumnName, DateTime.UtcNow);

            sqlQuery.SetParam(dictionaryParams);
        }

        /// <summary>
        /// Build the final `query statement and parameters`
        /// </summary>
        /// <param name="queryProperties"></param>
        /// <param name="sqlBuilder"></param>
        /// <param name="conditions"></param>
        /// <param name="qLevel">Parameters of the ranking</param>
        /// <remarks>
        /// Support `group conditions` syntax
        /// </remarks>
        private void BuildQuerySql(IList<QueryExpression> queryProperties,
            ref StringBuilder sqlBuilder, ref List<KeyValuePair<string, object>> conditions, ref int qLevel)
        {
            foreach (var expr in queryProperties)
            {
                if (!string.IsNullOrEmpty(expr.LinkingOperator))
                {
                    if (sqlBuilder.Length > 0)
                        sqlBuilder.Append(" ");

                    sqlBuilder
                        .Append(expr.LinkingOperator)
                        .Append(" ");
                }

                switch (expr)
                {
                    case QueryParameterExpression qpExpr:
                        var tableName = TableName;
                        string columnName;
                        if (qpExpr.NestedProperty)
                        {
                            var joinProperty = SqlJoinProperties.First(x => x.PropertyName == qpExpr.PropertyName);
                            tableName = string.IsNullOrEmpty(joinProperty.TableAlias) ? joinProperty.TableName : joinProperty.TableAlias;
                            columnName = joinProperty.ColumnName;
                        }
                        else
                        {
                            columnName = SqlProperties.First(x => x.PropertyName == qpExpr.PropertyName).ColumnName;
                        }

                        if (qpExpr.PropertyValue == null)
                        {
                            sqlBuilder.AppendFormat("{0}.{1} {2} NULL", tableName, columnName, qpExpr.QueryOperator == "=" ? "IS" : "IS NOT");
                        }
                        else
                        {
                            var vKey = string.Format("{0}_p{1}", qpExpr.PropertyName, qLevel); //Handle multiple uses of a field

                            sqlBuilder.AppendFormat("{0}.{1} {2} @{3}", tableName, columnName, qpExpr.QueryOperator, vKey);
                            conditions.Add(new KeyValuePair<string, object>(vKey, qpExpr.PropertyValue));
                        }

                        qLevel++;
                        break;

                    case QueryBinaryExpression qbExpr:
                        var nSqlBuilder = new StringBuilder();
                        var nConditions = new List<KeyValuePair<string, object>>();
                        BuildQuerySql(qbExpr.Nodes, ref nSqlBuilder, ref nConditions, ref qLevel);

                        if (qbExpr.Nodes.Count == 1) //Handle `grouping brackets`
                            sqlBuilder.Append(nSqlBuilder);
                        else
                            sqlBuilder.AppendFormat("({0})", nSqlBuilder);

                        conditions.AddRange(nConditions);
                        break;
                }
            }
        }

        /// <summary>
        /// Get query properties
        /// </summary>
        /// <param name="expr">The expression.</param>
        private List<QueryExpression> GetQueryProperties(Expression expr)
        {
            var queryNode = GetQueryProperties(expr, ExpressionType.Default);
            switch (queryNode)
            {
                case QueryParameterExpression qpExpr:
                    return new List<QueryExpression> {queryNode};

                case QueryBinaryExpression qbExpr:
                    return qbExpr.Nodes;

                default:
                    throw new NotSupportedException(queryNode.ToString());
            }
        }
    }
}
