using System.Collections.Generic;
using System.Text;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <summary>
    /// A result object with the generated sql and dynamic params.
    /// </summary>
    public class QueryResult
    {

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The param.</param>
        public QueryResult(string sql, dynamic param)
        {
            Param = param;
            Sql = sql;
        }


        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>
        /// The SQL.
        /// </value>
        public string Sql { get; private set; }

        /// <summary>
        /// Gets the param, for Select
        /// </summary>
        /// <value>
        /// The param.
        /// </value>
        public object Param { get; private set; }

        public void SetParam(dynamic param)
        {
            Param = param;
        }

        public void AppendToSql(string sqlString)
        {
            var sqlBuilder = new StringBuilder(Sql);
            sqlBuilder.AppendLine(sqlString);
            Sql = sqlBuilder.ToString();
        }

        public void AppendToSql(IEnumerable<string> sqlStrings)
        {
            var sqlBuilder = new StringBuilder(Sql);
            foreach (var s in sqlStrings)
            {
                sqlBuilder.AppendLine(s);
            }
            Sql = sqlBuilder.ToString();
        }

    }
}
