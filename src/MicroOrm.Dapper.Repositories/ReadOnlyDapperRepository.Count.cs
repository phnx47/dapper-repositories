using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
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
        public virtual int Count()
        {
            return Count(null, transaction: null);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync()
        {
            return CountAsync(transaction: null, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return CountAsync(transaction: null, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual int Count(IDbTransaction transaction)
        {
            return Count(null, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(IDbTransaction transaction)
        {
            return CountAsync(null, transaction, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            return CountAsync(null, transaction, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Count(predicate, transaction: null);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return CountAsync(predicate, transaction: null, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetCount(predicate);
            return Connection.QueryFirstOrDefault<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken)
        {
            return CountAsync(predicate, transaction: null, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
        {
            return CountAsync(predicate, transaction, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetCount(predicate);
            return Connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, object>> distinctField)
        {
            return Count(distinctField, transaction: null);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField)
        {
            return CountAsync(distinctField, transaction: null, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, CancellationToken cancellationToken)
        {
            return CountAsync(distinctField, transaction: null, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction)
        {
            return Count(null, distinctField, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction)
        {
            return CountAsync(null, distinctField, transaction, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            return CountAsync(null, distinctField, transaction, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField)
        {
            return Count(predicate, distinctField, transaction: null);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField)
        {
            return CountAsync(predicate, distinctField, transaction: null, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField, CancellationToken cancellationToken)
        {
            return CountAsync(predicate, distinctField, transaction: null, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField);
            return Connection.QueryFirstOrDefault<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction)
        {
            return CountAsync(predicate, distinctField, transaction, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField);
            return Connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }
    }
}
