using System.Data;

namespace MicroOrm.Dapper.Repositories.DbContext
{
   

    /// <summary>
    /// Class is helper for use and close IDbConnection
    /// </summary>
    public class DapperDbContext : IDapperDbContext
    {

        protected DapperDbContext(IDbConnection connection)
        {
            InnerConnection = connection;
        }

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

        public void Dispose()
        {
            if (InnerConnection != null && InnerConnection.State != ConnectionState.Closed)
                InnerConnection.Close();
        }
    }
}