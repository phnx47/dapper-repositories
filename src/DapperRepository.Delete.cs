using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace MicroOrm.Dapper.Repositories;

/// <summary>
///     Base Repository
/// </summary>
public partial class DapperRepository<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public virtual bool Delete(TEntity instance)
    {
        return Delete(instance, null, null);
    }

    /// <inheritdoc />
    public virtual bool Delete(TEntity instance, TimeSpan? timeout)
    {
        return Delete(instance, null, timeout);
    }

    /// <inheritdoc />
    public virtual bool Delete(TEntity instance, IDbTransaction? transaction, TimeSpan? timeout)
    {
        var queryResult = SqlGenerator.GetDelete(instance);
        int? commandTimeout = null;
        if (timeout.HasValue)
            commandTimeout = timeout.Value.Seconds;
        var deleted = Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout) > 0;
        return deleted;
    }

    /// <inheritdoc />
    public virtual bool Delete(Expression<Func<TEntity, bool>>? predicate)
    {
        return Delete(predicate, null, null);
    }

    /// <inheritdoc />
    public virtual bool Delete(Expression<Func<TEntity, bool>>? predicate, TimeSpan? timeout)
    {
        return Delete(predicate, null, timeout);
    }

    /// <inheritdoc />
    public virtual bool Delete(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, TimeSpan? timeout)
    {
        var queryResult = SqlGenerator.GetDelete(predicate);
        int? commandTimeout = null;
        if (timeout.HasValue)
            commandTimeout = timeout.Value.Seconds;
        var deleted = Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout) > 0;
        return deleted;
    }

    /// <inheritdoc />
    public virtual Task<bool> DeleteAsync(TEntity instance, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(instance, null, null, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<bool> DeleteAsync(TEntity instance, TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(instance, null, timeout, cancellationToken);
    }


    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(TEntity instance, IDbTransaction? transaction, TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        var queryResult = SqlGenerator.GetDelete(instance);
        int? commandTimeout = null;
        if (timeout.HasValue)
            commandTimeout = timeout.Value.Seconds;
        var deleted = await Connection.ExecuteAsync(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout,
            cancellationToken: cancellationToken)) > 0;
        return deleted;
    }

    /// <inheritdoc />
    public virtual Task<bool> DeleteAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(predicate, null, null, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<bool> DeleteAsync(Expression<Func<TEntity, bool>>? predicate, TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(predicate, null, timeout, cancellationToken);
    }


    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, TimeSpan? timeout,
        CancellationToken cancellationToken = default)
    {
        var queryResult = SqlGenerator.GetDelete(predicate);
        int? commandTimeout = null;
        if (timeout.HasValue)
            commandTimeout = timeout.Value.Seconds;
        var deleted = await Connection.ExecuteAsync(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout,
            cancellationToken: cancellationToken)) > 0;
        return deleted;
    }
}
