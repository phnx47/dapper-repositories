using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     interface for repository
    /// </summary>
    public interface IDapperRepository<TEntity> : IReadOnlyDapperRepository<TEntity> where TEntity : class
    {
        /// <summary>
        ///     Delete object from DB
        /// </summary>
        bool Delete(TEntity instance);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        bool Delete(TEntity instance, TimeSpan? timeout);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        bool Delete(TEntity instance, IDbTransaction? transaction, TimeSpan? timeout);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        bool Delete(Expression<Func<TEntity, bool>>? predicate);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        bool Delete(Expression<Func<TEntity, bool>>? predicate, TimeSpan? timeout);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        bool Delete(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, TimeSpan? timeout);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        Task<bool> DeleteAsync(TEntity instance, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        Task<bool> DeleteAsync(TEntity instance, TimeSpan? timeout, CancellationToken cancellationToken = default);


        /// <summary>
        ///     Delete object from DB
        /// </summary>
        Task<bool> DeleteAsync(TEntity instance, IDbTransaction? transaction, TimeSpan? timeout, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>>? predicate, TimeSpan? timeout, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, TimeSpan? timeout, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        bool Insert(TEntity instance);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        Task<bool> InsertAsync(TEntity instance);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        Task<bool> InsertAsync(TEntity instance, CancellationToken cancellationToken);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        bool Insert(TEntity instance, IDbTransaction? transaction);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        Task<bool> InsertAsync(TEntity instance, IDbTransaction? transaction);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        Task<bool> InsertAsync(TEntity instance, IDbTransaction? transaction, CancellationToken cancellationToken);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        int BulkInsert(IEnumerable<TEntity> instances);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        Task<int> BulkInsertAsync(IEnumerable<TEntity> instances);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, CancellationToken cancellationToken);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        int BulkInsert(IEnumerable<TEntity> instances, IDbTransaction? transaction);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction, CancellationToken cancellationToken);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(TEntity instance, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(TEntity instance, IDbTransaction? transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(TEntity instance, IDbTransaction? transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(TEntity instance, IDbTransaction? transaction, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(Expression<Func<TEntity, bool>>? predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Bulk Update objects to DB
        /// </summary>
        bool BulkUpdate(IEnumerable<TEntity> instances);

        /// <summary>
        ///     Bulk Update objects to DB
        /// </summary>
        Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances);

        /// <summary>
        ///     Bulk Update objects to DB
        /// </summary>
        Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, CancellationToken cancellationToken);

        /// <summary>
        ///     Bulk Update objects to DB
        /// </summary>
        bool BulkUpdate(IEnumerable<TEntity> instances, IDbTransaction? transaction);

        /// <summary>
        ///     Bulk Update objects to DB
        /// </summary>
        Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction);

        /// <summary>
        ///     Bulk Update objects to DB
        /// </summary>
        Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction, CancellationToken cancellationToken);
    }
}
