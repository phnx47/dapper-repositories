using System;
using System.Collections.Generic;
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
        public virtual IEnumerable<TEntity> FindAll(IDbTransaction transaction = null)
        {
            return FindAll(null, transaction);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate);
            return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }
        
        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(IDbTransaction transaction = null)
        {
            return FindAllAsync(null, transaction);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate);
            return Connection.QueryAsync<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }
    }
}
