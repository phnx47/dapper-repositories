using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace MicroOrm.Dapper.Repositories
{
    public partial interface IDapperRepository<TEntity>
    {
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
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes);

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
        Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes);

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

    public partial class DapperRepository<TEntity>
    {
        /// <inheritdoc />
        public virtual bool Insert(TEntity instance)
        {
            return Insert(instance, null);
        }

        /// <inheritdoc />
        public virtual Task<bool> InsertAsync(TEntity instance)
        {
            return InsertAsync(instance, null, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual Task<bool> InsertAsync(TEntity instance, CancellationToken cancellationToken)
        {
            return InsertAsync(instance, null, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<bool> InsertAsync(TEntity instance, IDbTransaction? transaction)
        {
            return InsertAsync(instance, transaction, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual int BulkInsert(IEnumerable<TEntity> instances)
        {
            return BulkInsert(instances, null);
        }

        /// <inheritdoc />
        public virtual Task<int> BulkInsertAsync(IEnumerable<TEntity> instances)
        {
            return BulkInsertAsync(instances, null, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, CancellationToken cancellationToken)
        {
            return BulkInsertAsync(instances, null, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> BulkInsertAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction)
        {
            return BulkInsertAsync(instances, transaction, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual bool Update(TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return Update(instance, null, includes);
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(instance, null, CancellationToken.None, includes);
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(TEntity instance, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(instance, null, cancellationToken, includes);
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(TEntity instance, IDbTransaction? transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(instance, transaction, CancellationToken.None, includes);
        }

        /// <inheritdoc />
        public virtual bool Update(Expression<Func<TEntity, bool>>? predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return Update(predicate, instance, null, includes);
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(predicate, instance, null, CancellationToken.None, includes);
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(predicate, instance, null, cancellationToken, includes);
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(Expression<Func<TEntity, bool>>? predicate, TEntity instance, IDbTransaction? transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(predicate, instance, transaction, CancellationToken.None, includes);
        }

        /// <inheritdoc />
        public virtual bool BulkUpdate(IEnumerable<TEntity> instances)
        {
            return BulkUpdate(instances, null);
        }

        /// <inheritdoc />
        public virtual Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances)
        {
            return BulkUpdateAsync(instances, null, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, CancellationToken cancellationToken)
        {
            return BulkUpdateAsync(instances, null, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<bool> BulkUpdateAsync(IEnumerable<TEntity> instances, IDbTransaction? transaction)
        {
            return BulkUpdateAsync(instances, transaction, CancellationToken.None);
        }

    }
}
