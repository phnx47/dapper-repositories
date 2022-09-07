using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual TEntity? FindById(object id)
        {
            return FindById(id, null);
        }

        /// <inheritdoc />
        public virtual TEntity? FindById(object id, IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetSelectById(id, null);
            return Connection.QuerySingleOrDefault<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<TEntity?> FindByIdAsync(object id)
        {
            return FindByIdAsync(id, null, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<TEntity?> FindByIdAsync(object id, CancellationToken cancellationToken)
        {
            return FindByIdAsync(id, null, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<TEntity?> FindByIdAsync(object id, IDbTransaction? transaction)
        {
            return FindByIdAsync(id, transaction, cancellationToken: default);
        }

        /// <inheritdoc />
        public virtual Task<TEntity?> FindByIdAsync(object id, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetSelectById(id, null);
            return Connection.QuerySingleOrDefaultAsync<TEntity?>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }
    }
}
