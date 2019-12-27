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
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindPaged(int offset, int limit)
        {
            return FindPaged(transaction: null, offset, limit);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindPaged(IDbTransaction transaction, int offset, int limit)
        {
            return FindPaged(null, offset, limit, transaction);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindPaged(Expression<Func<TEntity, bool>> predicate, int offset, int limit,
            IDbTransaction transaction)
        {
            var queryResult = SqlGenerator.GetSelectPaged(predicate, offset, limit);
            return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindPaged(Expression<Func<TEntity, bool>> predicate, int offset, int limit)
        {
            return FindPaged(predicate, offset, limit, null);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> predicate, int offset, int limit)
        {
            var queryResult = SqlGenerator.GetSelectPaged(predicate, offset, limit);
            return Connection.QueryAsync<TEntity>(queryResult.GetSql(), queryResult.Param, null);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindPagedAsync(int offset, int limit)
        {
            return FindPagedAsync(null, offset, limit, null);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindPagedAsync(IDbTransaction transaction, int offset, int limit)
        {
            return FindPagedAsync(null, offset, limit, transaction);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> predicate,
            int offset, int limit,
            IDbTransaction transaction)
        {
            var queryResult = SqlGenerator.GetSelectPaged(predicate, offset, limit);
            return Connection.QueryAsync<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }
    }
}
