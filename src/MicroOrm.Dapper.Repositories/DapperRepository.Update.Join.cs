using System;
using System.Data;
using System.Linq;
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
        public virtual bool Update<TChild1>(Expression<Func<TEntity, bool>> predicate, TEntity instance, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance, tChild1);
            var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }

        /// <inheritdoc />
        public virtual bool Update<TChild1>(TEntity instance, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction)
        {
            var sqlQuery = SqlGenerator.GetUpdate(instance, tChild1);
            var updated = Connection.Execute(sqlQuery.GetSql(), sqlQuery.Param, transaction) > 0;
            return updated;
        }
    }
}
