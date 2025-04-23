using System;
using System.Data;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories.DbContext;

/// <summary>
///     Wrapper for database connection management
/// </summary>
public interface IDapperDbContext : IDisposable
{
    /// <summary>
    ///     Get DB Connection, open connection if necessary
    /// </summary>
    IDbConnection Connection { get; }

    /// <summary>
    ///     Open DB connection if not already open
    /// </summary>
    void OpenConnection();

    /// <summary>
    ///     Open DB connection asynchronously if not already open
    /// </summary>
    Task OpenConnectionAsync();

    /// <summary>
    ///     Begin transaction, open connection if necessary
    /// </summary>
    IDbTransaction BeginTransaction();

    /// <summary>
    ///     Begin transaction asynchronously, open connection if necessary
    /// </summary>
    Task<IDbTransaction> BeginTransactionAsync();
}
