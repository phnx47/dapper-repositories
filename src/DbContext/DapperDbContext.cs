using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

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

    public async Task OpenConnectionAsync()
    {
        if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
        {
            if (InnerConnection is DbConnection dbConnection)
                await dbConnection.OpenAsync().ConfigureAwait(false);
            else
                InnerConnection.Open();
        }
    }

    public virtual IDbTransaction BeginTransaction()
    {
        return Connection.BeginTransaction();
    }

    public virtual async Task<IDbTransaction> BeginTransactionAsync()
    {
        if (InnerConnection is DbConnection dbConnection)
        {
            await OpenConnectionAsync().ConfigureAwait(false);
#if NETCOREAPP3_0_OR_GREATER
            return await dbConnection.BeginTransactionAsync().ConfigureAwait(false);
#else
            return dbConnection.BeginTransaction();
#endif
        }

        return BeginTransaction();
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
