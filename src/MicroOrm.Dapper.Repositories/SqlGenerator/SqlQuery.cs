using System.Text;

namespace MicroOrm.Dapper.Repositories.SqlGenerator
{
    /// <summary>
    ///     A object with the generated sql and dynamic params.
    /// </summary>
    public class SqlQuery
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        public SqlQuery()
        {
            SqlBuilder = new StringBuilder();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="param">The param.</param>
        public SqlQuery(object param)
            : this()
        {
            Param = param;
        }

        /// <summary>
        ///     SqlBuilder
        /// </summary>
        public StringBuilder SqlBuilder { get; }

        /// <summary>
        ///     Gets the param
        /// </summary>
        public object Param { get; private set; }

        /// <summary>
        ///     Gets the SQL.
        /// </summary>
        public string GetSql()
        {
            return SqlBuilder.ToString().Trim();
        }

        /// <summary>
        ///     Set alternative param
        /// </summary>
        /// <param name="param">The param.</param>
        public void SetParam(object param)
        {
            Param = param;
        }
    }
}