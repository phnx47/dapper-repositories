using System;
using System.Collections.Generic;
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
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        private const string _dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction)
        {
            var fromString = from.ToString(_dateTimeFormat);
            var toString = to.ToString(_dateTimeFormat);
            return FindAllBetween(fromString, toString, btwField, predicate, transaction);
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> FindAllBetween(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, FilterData, btwField, predicate);
            return Connection.Query<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            DateTime from,
            DateTime to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            return FindAllBetweenAsync(from.ToString(_dateTimeFormat), to.ToString(_dateTimeFormat), btwField, predicate, transaction, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> FindAllBetweenAsync(
            object from,
            object to,
            Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> predicate,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var queryResult = SqlGenerator.GetSelectBetween(from, to, FilterData, btwField, predicate);
            return Connection.QueryAsync<TEntity>(new CommandDefinition(queryResult.GetSql(), queryResult.Param, transaction, cancellationToken: cancellationToken));
        }
    }
}
