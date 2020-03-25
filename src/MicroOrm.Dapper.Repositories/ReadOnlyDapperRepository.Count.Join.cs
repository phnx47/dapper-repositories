using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;

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
        public virtual int Count(params Expression<Func<TEntity, object>>[] includes)
        {
            return Count(transaction: null);
        }

        /// <inheritdoc />
        public virtual int Count(IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            return Count(null, transaction, includes: includes);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return Count(predicate, transaction: null, includes: includes);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var queryResult = SqlGenerator.GetCount(predicate, includes);
            return Connection.QueryFirstOrDefault<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, object>> distinctField, params Expression<Func<TEntity, object>>[] includes)
        {
            return Count(distinctField, null, includes: includes);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            return Count(null, distinctField, transaction);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, params Expression<Func<TEntity, object>>[] includes)
        {
            return Count(predicate, distinctField, null, includes);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField, includes);
            return Connection.QueryFirstOrDefault<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            return CountAsync(transaction: null, includes: includes);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            return CountAsync(null, transaction, includes: includes);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return CountAsync(predicate, transaction: null, includes: includes);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var queryResult = SqlGenerator.GetCount(predicate, includes);
            return Connection.QueryFirstOrDefaultAsync<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, params Expression<Func<TEntity, object>>[] includes)
        {
            return CountAsync(distinctField, null, includes: includes);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            return CountAsync(null, distinctField, transaction, includes: includes);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return CountAsync(predicate, distinctField, null, includes: includes);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField, includes);
            return Connection.QueryFirstOrDefaultAsync<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }
    }
}
