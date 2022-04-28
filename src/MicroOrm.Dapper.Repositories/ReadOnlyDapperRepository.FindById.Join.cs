using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories
{
    /// <inheritdoc />
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual TEntity? FindById<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1);
            return ExecuteJoinQuery<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TEntity? FindById<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2);
            return ExecuteJoinQuery<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual TEntity? FindById<TChild1, TChild2, TChild3>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2, tChild3);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1, tChild2, tChild3).FirstOrDefault();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual async Task<TEntity?> FindByIdAsync<TChild1>(object id,
            Expression<Func<TEntity, object>> tChild1,
            IDbTransaction? transaction,
            CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1);
            return (await ExecuteJoinQueryAsync<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1)).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity?> FindByIdAsync<TChild1, TChild2>(object id,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction? transaction,
            CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetSelectById(id, FilterData, tChild1, tChild2);
            return (await ExecuteJoinQueryAsync<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, cancellationToken, tChild1, tChild2)).FirstOrDefault();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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
}
