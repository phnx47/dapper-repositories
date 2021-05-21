using System;
using System.Data;
using System.Linq.Expressions;
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
        public virtual bool Delete(TEntity instance, IDbTransaction transaction = null, TimeSpan? timeout = null)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            int? commandTimeout = null;
            if (timeout.HasValue)
                commandTimeout = timeout.Value.Seconds;
            var deleted = Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout) > 0;
            return deleted;
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(TEntity instance, IDbTransaction transaction = null, TimeSpan? timeout = null)
        {
            var queryResult = SqlGenerator.GetDelete(instance);
            int? commandTimeout = null;
            if (timeout.HasValue)
                commandTimeout = timeout.Value.Seconds;
            var deleted = await Connection.ExecuteAsync(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout) > 0;
            return deleted;
        }

        /// <inheritdoc />
        public virtual bool Delete(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, TimeSpan? timeout = null)
        {
            var queryResult = SqlGenerator.GetDelete(predicate);
            int? commandTimeout = null;
            if (timeout.HasValue)
                commandTimeout = timeout.Value.Seconds;
            var deleted = Connection.Execute(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout) > 0;
            return deleted;
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, TimeSpan? timeout = null)
        {
            var queryResult = SqlGenerator.GetDelete(predicate);
            int? commandTimeout = null;
            if (timeout.HasValue)
                commandTimeout = timeout.Value.Seconds;
            var deleted = await Connection.ExecuteAsync(queryResult.GetSql(), queryResult.Param, transaction, commandTimeout) > 0;
            return deleted;
        }
    }
}
