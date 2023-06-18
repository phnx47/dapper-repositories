using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace MicroOrm.Dapper.Repositories;

/// <summary>
///     Base Repository
/// </summary>
public partial class ReadOnlyDapperRepository<TEntity>
    where TEntity : class
{

    public virtual IEnumerable<TEntity> FindAll()
    {
        return FindAll(null, transaction: null);
    }


    public virtual Task<IEnumerable<TEntity>> FindAllAsync()
    {
        return FindAllAsync(null, transaction: null, cancellationToken: default);
    }


    public virtual Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken)
    {
        return FindAllAsync(null, transaction: null, cancellationToken: cancellationToken);
    }


    public virtual IEnumerable<TEntity> FindAll(IDbTransaction transaction)
    {
        return FindAll(null, transaction);
    }


    public virtual Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction)
    {
        return FindAllAsync(null, transaction, cancellationToken: default);
    }


    public virtual Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        return FindAllAsync(null, transaction, cancellationToken: cancellationToken);
    }


    public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
    {
        return FindAll(predicate, transaction: null);
    }


    public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return FindAllAsync(predicate, transaction: null, cancellationToken: default);
    }


    public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken)
    {
        return FindAllAsync(predicate, transaction: null, cancellationToken: cancellationToken);
    }


    public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, string, string>> orderBy)
    {
        return FindAll(predicate, transaction: null);
    }


    public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectAll(predicate, FilterData);
        return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
    }


    public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
    {
        return FindAllAsync(predicate, transaction, cancellationToken: default);
    }


    public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectAll(predicate, FilterData);
        return await Connection.QueryAsync<TEntity>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
    }
}
