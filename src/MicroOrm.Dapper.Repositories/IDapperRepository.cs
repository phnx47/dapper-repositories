using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     interface for repository
    /// </summary>
    public interface IDapperRepository<TEntity> : IReadOnlyDapperRepository<TEntity> where TEntity : class
    {
        /// <summary>
        ///     Insert object to DB
        /// </summary>
        bool Insert(TEntity instance);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        bool Insert(TEntity instance, IDbTransaction transaction);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        Task<bool> InsertAsync(TEntity instance);

        /// <summary>
        ///     Insert object to DB
        /// </summary>
        Task<bool> InsertAsync(TEntity instance, IDbTransaction transaction);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        int BulkInsert(IEnumerable<TEntity> instances, IDbTransaction transaction = null);

        /// <summary>
        ///     Bulk Insert objects to DB
        /// </summary>
        Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, IDbTransaction transaction = null);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        bool Delete(TEntity instance, IDbTransaction transaction = null, TimeSpan? timeout = null);

        /// <summary>
        ///     Delete object from DB
        /// </summary>
        Task<bool> DeleteAsync(TEntity instance, IDbTransaction transaction = null, TimeSpan? timeout = null);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        bool Delete(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, TimeSpan? timeout = null);

        /// <summary>
        ///     Delete objects from DB
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, TimeSpan? timeout = null);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(Expression<Func<TEntity, bool>> predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        bool Update(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Update object in DB
        /// </summary>
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        ///     Bulk Update objects in DB
        /// </summary>
        Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances);

        /// <summary>
        ///     Bulk Update objects in DB
        /// </summary>
        Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, IDbTransaction transaction);

        /// <summary>
        ///     Bulk Update objects in DB
        /// </summary>
        bool BulkUpdate(IEnumerable<TEntity> instances);

        /// <summary>
        ///     Bulk Update objects in DB
        /// </summary>
        bool BulkUpdate(IEnumerable<TEntity> instances, IDbTransaction transaction);
    }
}
