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

    public virtual TEntity? FindById<TChild1>(object id,
        Expression<Func<TEntity, object>> tChild1)
    {
        return FindById<TChild1>(id, tChild1, null);
    }


    public virtual TEntity? FindById<TChild1>(object id,
        Expression<Func<TEntity, object>> tChild1,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1);
        return ExecuteJoinQuery<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1).FirstOrDefault();
    }


    public virtual TEntity? FindById<TChild1, TChild2>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2)
    {
        return FindById<TChild1, TChild2>(id, tChild1, tChild2, null);
    }


    public virtual TEntity? FindById<TChild1, TChild2>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2);
        return ExecuteJoinQuery<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2).FirstOrDefault();
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3)
    {
        return FindById<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, null);
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3).FirstOrDefault();
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3, TChild4>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4)
    {
        return FindById<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, null);
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3, TChild4>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3, tChild4);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4).FirstOrDefault();
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5)
    {
        return FindById<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, null);
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5)
            .FirstOrDefault();
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6)
    {
        return FindById<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null);
    }


    public virtual TEntity? FindById<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        IDbTransaction? transaction)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(queryResult, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6)
            .FirstOrDefault();
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1>(object id,
        Expression<Func<TEntity, object>> tChild1)
    {
        return FindByIdAsync<TChild1>(id, tChild1, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1>(object id,
        Expression<Func<TEntity, object>> tChild1,
        CancellationToken cancellationToken)
    {
        return FindByIdAsync<TChild1>(id, tChild1, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1>(object id,
        Expression<Func<TEntity, object>> tChild1,
        IDbTransaction? transaction)
    {
        return FindByIdAsync<TChild1>(id, tChild1, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindByIdAsync<TChild1>(object id,
        Expression<Func<TEntity, object>> tChild1,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1);
        return (await ExecuteJoinQueryAsync<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1)).FirstOrDefault();
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2)
    {
        return FindByIdAsync<TChild1, TChild2>(id, tChild1, tChild2, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        CancellationToken cancellationToken)
    {
        return FindByIdAsync<TChild1, TChild2>(id, tChild1, tChild2, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        IDbTransaction? transaction)
    {
        return FindByIdAsync<TChild1, TChild2>(id, tChild1, tChild2, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindByIdAsync<TChild1, TChild2>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2)).FirstOrDefault();
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        CancellationToken cancellationToken)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        IDbTransaction? transaction)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3>(id, tChild1, tChild2, tChild3, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3)).FirstOrDefault();
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        CancellationToken cancellationToken)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        IDbTransaction? transaction)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(id, tChild1, tChild2, tChild3, tChild4, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3, tChild4);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3, tChild4))
            .FirstOrDefault();
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        CancellationToken cancellationToken)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        IDbTransaction? transaction)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(id, tChild1, tChild2, tChild3, tChild4, tChild5, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3, tChild4, tChild5))
            .FirstOrDefault();
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: default);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        CancellationToken cancellationToken)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, null, cancellationToken: cancellationToken);
    }


    public virtual Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        IDbTransaction? transaction)
    {
        return FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(id, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6, transaction, cancellationToken: default);
    }


    public virtual async Task<TEntity?> FindByIdAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(object id,
        Expression<Func<TEntity, object>> tChild1,
        Expression<Func<TEntity, object>> tChild2,
        Expression<Func<TEntity, object>> tChild3,
        Expression<Func<TEntity, object>> tChild4,
        Expression<Func<TEntity, object>> tChild5,
        Expression<Func<TEntity, object>> tChild6,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        return (await ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(queryResult, transaction, cancellationToken, tChild1, tChild2, tChild3, tChild4, tChild5,
            tChild6)).FirstOrDefault();
    }
}
