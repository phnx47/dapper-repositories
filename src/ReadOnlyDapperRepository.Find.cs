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
public partial class ReadOnlyDapperRepository<TEntity>
    where TEntity : class
{

    public virtual TEntity? Find()
    {
        return Find(null, null);
    }


    public virtual Task<TEntity?> FindAsync()
    {
        return FindAsync(null, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync(CancellationToken cancellationToken)
    {
        return FindAsync(null, null, cancellationToken: cancellationToken);
    }


    public virtual TEntity? Find(IDbTransaction transaction)
    {
        return Find(null, transaction);
    }


    public virtual Task<TEntity?> FindAsync(IDbTransaction transaction)
    {
        return FindAsync(null, transaction, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync(IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        return FindAsync(null, transaction, cancellationToken: cancellationToken);
    }


    public virtual TEntity? Find(Expression<Func<TEntity, bool>> predicate)
    {
        return Find(predicate, null);
    }


    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return FindAsync(predicate, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken)
    {
        return FindAsync(predicate, null, cancellationToken: cancellationToken);
    }


    public virtual TEntity? Find(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData);
        return Connection.QueryFirstOrDefault<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
    }


    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
    {
        return FindAsync(predicate, transaction, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData);
        return Connection.QueryFirstOrDefaultAsync<TEntity?>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
    }
}
