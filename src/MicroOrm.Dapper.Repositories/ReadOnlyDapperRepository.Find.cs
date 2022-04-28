using System;
using System.Data;
using System.Linq.Expressions;
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
        public virtual TEntity? Find(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData);
            return Connection.QueryFirstOrDefault<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData);
            return Connection.QueryFirstOrDefaultAsync<TEntity?>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }
    }
}
