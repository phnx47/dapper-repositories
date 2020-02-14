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
        public virtual bool Update(TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return Update(instance, null, includes);
        }

        /// <inheritdoc />
        public virtual bool Update(TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance,includes);
            var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(instance, null, includes);
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance,includes);
            var updated = await Connection.ExecuteAsync(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual bool Update(Expression<Func<TEntity, bool>> predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return Update(predicate, instance, null, includes);
        }

        /// <inheritdoc />
        public virtual bool Update(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance, includes);
            var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity instance, params Expression<Func<TEntity, object>>[] includes)
        {
            return UpdateAsync(predicate, instance, null, includes);
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction, params Expression<Func<TEntity, object>>[] includes)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance, includes);
            var updated = await Connection.ExecuteAsync(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }
    }
}
