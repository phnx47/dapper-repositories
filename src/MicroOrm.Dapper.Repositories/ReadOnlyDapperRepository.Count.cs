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
        public virtual int Count(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetCount(predicate);
            return Connection.QueryFirstOrDefault<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField);
            return Connection.QueryFirstOrDefault<int>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetCount(predicate);
            return Connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, object>> distinctField, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetCount(predicate, distinctField);
            return Connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }
    }
}
