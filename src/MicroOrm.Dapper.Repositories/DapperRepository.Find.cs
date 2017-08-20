using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;


namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class DapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual TEntity Find()
        {
            return Find(null, null);
        }

        /// <inheritdoc />
        public virtual TEntity Find(IDbTransaction transaction)
        {
            return Find(null, transaction);
        }

        /// <inheritdoc />
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Find(predicate, null);
        }

        /// <inheritdoc />
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction)
        {
            return FindAll(predicate, transaction).FirstOrDefault();
        }
    }
}
