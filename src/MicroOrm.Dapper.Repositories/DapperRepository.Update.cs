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
    public partial class DapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual bool Update(TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance, includes);
            var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(TEntity instance, IDbTransaction transaction, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance, includes);
            var updated = await Connection.ExecuteAsync(new CommandDefinition(sqlQuery.GetSql(), sqlQuery.Param, transaction, cancellationToken: cancellationToken)) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual bool Update(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance, includes);
            var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction,
            CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance, includes);
            var updated = await Connection.ExecuteAsync(new CommandDefinition(sqlQuery.GetSql(), sqlQuery.Param, transaction, cancellationToken: cancellationToken)) > 0;
            return updated;
        }
    }
}
