using System;
using System.Collections.Generic;
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
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate,
            Expression<Func<TEntity, string, string>> orderBy)
        {
            return FindAll(predicate, transaction: null);
        }
        
        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, FilterData);
            return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate, IDbTransaction? transaction, CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, FilterData);
            return await Connection.QueryAsync<TEntity>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }
    }
}
