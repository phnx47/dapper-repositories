using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     interface for repository
    /// </summary>
    public partial interface IDapperRepository<TEntity> : IReadOnlyDapperRepository<TEntity> where TEntity : class
    {
        /// <summary>
        ///     Delete object from DB
        /// </summary>
        bool Delete(TEntity instance, IDbTransaction transaction = null, TimeSpan? timeout = null);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        Task<bool> DeleteAsync(TEntity instance, IDbTransaction transaction, TimeSpan? timeout);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        Task<bool> DeleteAsync(TEntity instance, IDbTransaction transaction = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        bool Delete(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, TimeSpan? timeout = null);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction, TimeSpan? timeout);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
    }
}
