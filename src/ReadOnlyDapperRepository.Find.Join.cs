using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories;

/// <summary>
///     Base Repository
/// </summary>
public partial class ReadOnlyDapperRepository<TEntity>
    where TEntity : class
{

    public virtual TEntity? Find<TChild1>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1)
    {
        return Find<TChild1>(predicate, tChild1, null);
    }


    public virtual TEntity? Find<TChild1>(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1);
        return ExecuteJoinQuery<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1).FirstOrDefault();
    }


    public virtual TEntity? Find<TChild1, TChild2>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2)
    {
        return Find<TChild1, TChild2>(predicate, tChild1, tChild2, null);
    }


    public virtual TEntity? Find<TChild1, TChild2>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2);
        return ExecuteJoinQuery<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2).FirstOrDefault();
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3)
    {
        return Find<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null);
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3).FirstOrDefault();
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4)
    {
        return Find<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null);
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3, tChild4);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4).FirstOrDefault();
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5)
    {
        return Find<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null);
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5).FirstOrDefault();
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6)
    {
        return Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null);
    }


    public virtual TEntity? Find<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6)
            .FirstOrDefault();
    }


    public virtual Task<TEntity?> FindAsync<TChild1>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1)
    {
        return FindAsync<TChild1>(predicate, tChild1, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync<TChild1>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        CancellationToken cancellationToken)
    {
        return FindAsync<TChild1>(predicate, tChild1, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindAsync<TChild1>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        IDbTransaction? transaction)
    {
        return FindAsync<TChild1>(predicate, tChild1, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindAsync<TChild1>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1);
        return (await ExecuteJoinQueryAsync<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1).ConfigureAwait(false)).FirstOrDefault();
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2)
    {
        return FindAsync<TChild1, TChild2>(predicate, tChild1, tChild2, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        CancellationToken cancellationToken)
    {
        return FindAsync<TChild1, TChild2>(predicate, tChild1, tChild2, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        IDbTransaction? transaction)
    {
        return FindAsync<TChild1, TChild2>(predicate, tChild1, tChild2, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindAsync<TChild1, TChild2>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2).ConfigureAwait(false)).FirstOrDefault();
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3)
    {
        return FindAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        CancellationToken cancellationToken)
    {
        return FindAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        IDbTransaction? transaction)
    {
        return FindAsync<TChild1, TChild2, TChild3>(predicate, tChild1, tChild2, tChild3, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindAsync<TChild1, TChild2, TChild3>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3).ConfigureAwait(false)).FirstOrDefault();
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        CancellationToken cancellationToken)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        IDbTransaction? transaction)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4>(predicate, tChild1, tChild2, tChild3, tChild4, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3, tChild4);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3, tChild4).ConfigureAwait(false))
            .FirstOrDefault();
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        CancellationToken cancellationToken)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        IDbTransaction? transaction)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3, tChild4, tChild5).ConfigureAwait(false))
            .FirstOrDefault();
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        CancellationToken cancellationToken)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        IDbTransaction? transaction)
    {
        return FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3, tChild4, tChild5,
            tChild6).ConfigureAwait(false)).FirstOrDefault();
    }
}
