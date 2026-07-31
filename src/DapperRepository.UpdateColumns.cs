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
    public virtual bool UpdateColumns(TEntity instance, Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumns(instance, null, column, columns);
    }

    public virtual Task<bool> UpdateColumnsAsync(TEntity instance, Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumnsAsync(instance, null, CancellationToken.None, column, columns);
    }

    public virtual Task<bool> UpdateColumnsAsync(TEntity instance, CancellationToken cancellationToken, Expression<Func<TEntity, object>> column,
        params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumnsAsync(instance, null, cancellationToken, column, columns);
    }

    public virtual bool UpdateColumns(TEntity instance, IDbTransaction? transaction, Expression<Func<TEntity, object>> column,
        params Expression<Func<TEntity, object>>[] columns)
    {
        var sqlQuery = SqlGenerator.GetUpdateColumns(instance, column, columns);
        var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
        return updated;
    }

    public virtual Task<bool> UpdateColumnsAsync(TEntity instance, IDbTransaction? transaction, Expression<Func<TEntity, object>> column,
        params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumnsAsync(instance, transaction, CancellationToken.None, column, columns);
    }

    public virtual async Task<bool> UpdateColumnsAsync(TEntity instance, IDbTransaction? transaction, CancellationToken cancellationToken,
        Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        var sqlQuery = SqlGenerator.GetUpdateColumns(instance, column, columns);
        var updated = await Connection.ExecuteAsync(new CommandDefinition(sqlQuery.GetSql(), sqlQuery.Param, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false) > 0;
        return updated;
    }

    public virtual bool UpdateColumns(Expression<Func<TEntity, bool>>? predicate, TEntity instance, Expression<Func<TEntity, object>> column,
        params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumns(predicate, instance, null, column, columns);
    }

    public virtual Task<bool> UpdateColumnsAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, Expression<Func<TEntity, object>> column,
        params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumnsAsync(predicate, instance, CancellationToken.None, column, columns);
    }

    public virtual Task<bool> UpdateColumnsAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, CancellationToken cancellationToken,
        Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumnsAsync(predicate, instance, null, cancellationToken, column, columns);
    }

    public virtual bool UpdateColumns(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction,
        Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        var sqlQuery = SqlGenerator.GetUpdateColumns(predicate, instance, column, columns);
        var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
        return updated;
    }

    public virtual Task<bool> UpdateColumnsAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction,
        Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        return UpdateColumnsAsync(predicate, instance, transaction, CancellationToken.None, column, columns);
    }

    public virtual async Task<bool> UpdateColumnsAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction,
        CancellationToken cancellationToken, Expression<Func<TEntity, object>> column, params Expression<Func<TEntity, object>>[] columns)
    {
        var sqlQuery = SqlGenerator.GetUpdateColumns(predicate, instance, column, columns);
        var updated = await Connection.ExecuteAsync(new CommandDefinition(sqlQuery.GetSql(), sqlQuery.Param, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false) > 0;
        return updated;
    }
}
