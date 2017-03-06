using System.Data;

namespace MicroOrm.Dapper.Repositories.DbContext
{
    /// <summary>
    ///     Class is helper for use and close IDbConnection
    /// </summary>
    public class DapperDbContext : IDapperDbContext
    {
        /// <summary>
        ///     DB Connection for internal use
        /// </summary>
        protected readonly IDbConnection InnerConnection;

        /// <summary>
        ///     Constructor
        /// </summary>
        protected DapperDbContext(IDbConnection connection)
        {
            InnerConnection = connection;
        }

        /// <summary>
        ///     Get opened DB Connection
        /// </summary>
        public virtual IDbConnection Connection
        {
            get
            {
                OpenConnection();
                return InnerConnection;
            }
        }

        /// <summary>
        ///     Open DB connection
        /// </summary>
        public void OpenConnection()
        {
            if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
                InnerConnection.Open();
        }

        /// <summary>
        ///     Open DB connection and Begin transaction
        /// </summary>
        public virtual IDbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        /// <summary>
        ///     Close DB connection
        /// </summary>
        public void Dispose()
        {
            if (InnerConnection != null && InnerConnection.State != ConnectionState.Closed)
                InnerConnection.Close();
        }
    }
}