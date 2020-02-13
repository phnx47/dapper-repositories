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
        public virtual TEntity Update<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, TEntity instance, IDbTransaction transaction)
        {
            var sqlQuery = SqlGenerator.GetUpdate(predicate, instance);
            var queryResult = SqlGenerator.GetSelectFirst(predicate, FilterData, tChild1);
            return ExecuteJoinQuery<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1).FirstOrDefault();
        }
    }
}
