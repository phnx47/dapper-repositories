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
        public virtual IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, tChild1);
            return ExecuteJoinQuery<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2);
            return ExecuteJoinQuery<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAll<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
            return ExecuteJoinQuery<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> tChild1, IDbTransaction transaction = null)
        {
            var queryResult = SqlGenerator.GetSelectAll(predicate, tChild1);
            return ExecuteJoinQueryAsync<TChild1, DontMap, DontMap, DontMap, DontMap, DontMap>(queryResult, transaction, tChild1);
        }


        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2);
            return ExecuteJoinQueryAsync<TChild1, TChild2, DontMap, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, DontMap, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, DontMap, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, DontMap>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> tChild1,
            Expression<Func<TEntity, object>> tChild2,
            Expression<Func<TEntity, object>> tChild3,
            Expression<Func<TEntity, object>> tChild4,
            Expression<Func<TEntity, object>> tChild5,
            Expression<Func<TEntity, object>> tChild6,
            IDbTransaction transaction = null)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
            return ExecuteJoinQueryAsync<TChild1, TChild2, TChild3, TChild4, TChild5, TChild6>(sqlQuery, transaction, tChild1, tChild2, tChild3, tChild4, tChild5, tChild6);
        }
    }
}
