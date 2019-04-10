using System;
using System.Collections.Generic;
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
            IDictionary<string, object> dictionaryParams = new Dictionary<string, object>();

            if (predicate != null)
            {
                // WHERE
                var queryProperties = new List<QueryExpression>();
                FillQueryProperties(predicate.Body, ref queryProperties);

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
    }
}
