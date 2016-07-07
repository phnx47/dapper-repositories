#if COREFX
using IDbConnection = System.Data.Common.DbConnection;
using IDbTransaction = System.Data.Common.DbTransaction;
#endif

using System;
using System.Data;

namespace MicroOrm.Dapper.Repositories.DbContext
{
   

    /// <summary>
    /// Class is helper for use and close IDbConnection
    /// </summary>
    public class DapperDbContext : IDapperDbContext
    {
        protected DapperDbContext(IDbConnection connection, IDbTransaction transaction = null)
        {
            InnerConnection = connection;
            InnerTransaction = transaction;
        }

        protected readonly IDbConnection InnerConnection;
        protected IDbTransaction InnerTransaction;

        public virtual IDbConnection Connection
        {
            get
            {
                OpenConnection();
                return InnerConnection;
            }
        }
        public virtual IDbTransaction Transaction
        {
            get { return InnerTransaction; }
            set { InnerTransaction = value; }
        }

        public virtual void OpenConnection()
        {
            if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
                InnerConnection.Open();
        }

        public virtual IDbTransaction BeginTransaction()
        {
            OpenConnection();
            InnerTransaction = InnerConnection.BeginTransaction();
            return InnerTransaction;
        }
        public void Dispose()
        {
            if (InnerConnection != null && InnerConnection.State != ConnectionState.Closed)
                InnerConnection.Close();
        }
    }
}