using System.Data;

namespace MicroOrm.Dapper.Repositories.DbContext;

public class DapperDbContext : IDapperDbContext
{
    /// <summary>
    ///     Internal DB connection instance
    /// </summary>
    protected readonly IDbConnection InnerConnection;

    /// <summary>
    ///     Create new DB context with connection
    /// </summary>
    protected DapperDbContext(IDbConnection connection)
    {
        InnerConnection = connection;
    }

    public virtual IDbConnection Connection
    {
        get
        {
            OpenConnection();
            return InnerConnection;
        }
    }

    public void OpenConnection()
    {
        if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
            InnerConnection.Open();
    }

    public virtual IDbTransaction BeginTransaction()
    {
        return Connection.BeginTransaction();
    }

    /// <summary>
    ///     Close DB connection
    /// </summary>
    public void Dispose()
    {
        if (InnerConnection.State != ConnectionState.Closed)
            InnerConnection.Close();
    }
}
