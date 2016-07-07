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
        protected DapperDbContext(IDbConnection connection, IDbTransaction transaction = null)
        {
            InnerConnection = connection;
            InnerTransaction = transaction;
        }

        /// <summary>
        /// 
        /// </summary>
        protected readonly IDbConnection InnerConnection;

        /// <summary>
        /// 
        /// </summary>
        protected IDbTransaction InnerTransaction;


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

        /// <summary>
        /// 
        /// </summary>
        public virtual IDbTransaction Transaction
        {
            get { return InnerTransaction; }
            set { InnerTransaction = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OpenConnection()
        {
            if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
                InnerConnection.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IDbTransaction BeginTransaction()
        {
            OpenConnection();
            InnerTransaction = InnerConnection.BeginTransaction();
            return InnerTransaction;
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