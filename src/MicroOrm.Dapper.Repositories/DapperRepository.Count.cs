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
    public partial class DapperRepository<TEntity>
        where TEntity : class
    {

        /// <inheritdoc />
        public virtual int Count(IDbTransaction transaction = null)
        {
            return Count(null, transaction);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetCount(predicate);
            return Connection.ExecuteScalar<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction = null)
        {
            return Count(null, distinctField, transaction);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField);
            return Connection.ExecuteScalar<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(IDbTransaction transaction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CountAsync(null, transaction, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, CancellationToken cancellationToken = default (CancellationToken))
        {
            var queryResult = SqlGenerator.GetCount(predicate);
            var cmdDefinition = new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction,
                cancellationToken: cancellationToken);
            return Connection.ExecuteScalarAsync<int>(cmdDefinition);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CountAsync(null, distinctField, transaction, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> distinctField, IDbTransaction transaction = null, CancellationToken cancellationToken = default (CancellationToken))
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField);
            var cmdDefinition = new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction,
                cancellationToken: cancellationToken);
            return Connection.ExecuteScalarAsync<int>(cmdDefinition);
        }
    }
}
