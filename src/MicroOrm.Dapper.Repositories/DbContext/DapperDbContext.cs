using System;
using System.Data;

namespace MicroOrm.Dapper.Repositories.DbContext
{
    /// <summary>
    /// Class is helper for use and close IDbConnection
    /// </summary>
    public abstract class DapperDbContext : IDisposable
    {
        #region Constructors

        protected DapperDbContext(IDbConnection connection)
        {
            InnerConnection = connection;
        }

        #endregion Constructors

        #region Properties

        protected readonly IDbConnection InnerConnection;

        public virtual IDbConnection Connection
        {
            get
            {
                if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
                    InnerConnection.Open();

                return InnerConnection;
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            if (InnerConnection != null && InnerConnection.State != ConnectionState.Closed)
                InnerConnection.Close();
        }

        #endregion Methods
    }
}