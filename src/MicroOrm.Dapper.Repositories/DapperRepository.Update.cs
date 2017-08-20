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
        public virtual bool Update(TEntity instance)
        {
            return Update(instance, null);
        }

        /// <inheritdoc />
        public virtual bool Update(TEntity instance, IDbTransaction transaction)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance);
            var updated = Connection.Execute(sqlQuery.GetSql(), instance, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(TEntity instance)
        {
            return UpdateAsync(instance, null);
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(TEntity instance, IDbTransaction transaction)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance);
            var updated = await Connection.ExecuteAsync(sqlQuery.GetSql(), instance, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual bool Update(Expression<Func<TEntity, bool>> predicate, TEntity instance)
        {
            return Update(predicate, instance, null);
        }

        /// <inheritdoc />
        public virtual bool Update(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance);
            var updated = Connection.Execute(sqlQuery.GetSql(), instance, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity instance)
        {
            return UpdateAsync(predicate, instance, null);
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity instance, IDbTransaction transaction)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance);
            var updated = await Connection.ExecuteAsync(sqlQuery.GetSql(), instance, transaction) > 0;
            return updated;
        }
    }
}