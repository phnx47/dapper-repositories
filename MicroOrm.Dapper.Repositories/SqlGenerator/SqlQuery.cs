using System.Collections.Generic;
using System.Text;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <summary>
    /// A object with the generated sql and dynamic params.
    /// </summary>
    public class SqlQuery
    {

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The param.</param>
        public SqlQuery(string sql, dynamic param)
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

        /// <summary>
        /// Set alternative param
        /// </summary>
        /// <param name="param"></param>
        public void SetParam(dynamic param)
        {
            Param = param;
        }


        /// <summary>
        /// Append string in current SQL query
        /// </summary>
        public void AppendToSql(string sqlString)
        {
            var sqlBuilder = new StringBuilder(Sql);
            sqlBuilder.Append(sqlString);
            Sql = sqlBuilder.ToString();
        }


        /// <summary>
        /// Append string in current SQL query
        /// </summary>
        public void AppendToSql(IEnumerable<string> sqlStrings)
        {
            var sqlBuilder = new StringBuilder(Sql);
            foreach (var s in sqlStrings)
            {
                sqlBuilder.Append(s);
            }
            Sql = sqlBuilder.ToString();
        }

    }
}
