using System.Data;

namespace MicroOrm.Dapper.Repositories.DbContext
{

    /// <summary>
    /// Class is helper for use and close IDbConnection
    /// </summary>
    public class DapperDbContext : IDapperDbContext
    {
        /// <summary>
        /// 
        /// </summary>
        protected DapperDbContext(IDbConnection connection)
        {
            InnerConnection = connection;
        }

        /// <summary>
        /// 
        /// </summary>
        protected readonly IDbConnection InnerConnection;

        /// <summary>
        /// 
        /// </summary>
        public virtual IDbConnection Connection
        {
            get
            {
                OpenConnection();
                return InnerConnection;
            }
        }


        private void OpenConnection()
        {
            if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
                InnerConnection.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IDbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (InnerConnection != null && InnerConnection.State != ConnectionState.Closed)
                InnerConnection.Close();
        }
    }
}