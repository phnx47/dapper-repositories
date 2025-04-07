using System;
using System.Data;
using System.Threading.Tasks;
namespace MicroOrm.Dapper.Repositories.DbContext;

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

    //Summary Open Db connection and begin transaction asynchronously
    public async Task OpenConnectionAsync()
    {
        try
        {
            if (InnerConnection.State != ConnectionState.Open && InnerConnection.State != ConnectionState.Connecting)
                await Task.Run(() => InnerConnection.Open()).ConfigureAwait(false);
        }

        catch (Exception )
        {
            throw;
        }
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        try
        {
            await OpenConnectionAsync().ConfigureAwait(false);
            return await Task.Run(() => Connection.BeginTransaction()).ConfigureAwait(false);
        }
        catch (Exception )
        {
            throw;
        }
    }

    /// <summary>
    ///     Close DB connection
    /// </summary>
    ///


    public void Dispose()
    {
        if (InnerConnection.State != ConnectionState.Closed)
            InnerConnection.Close();
    }
}
