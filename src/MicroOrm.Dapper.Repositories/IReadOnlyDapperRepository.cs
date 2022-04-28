using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     interface for read only repository
    /// </summary>
    public partial interface IReadOnlyDapperRepository<TEntity> : IDisposable where TEntity : class
    {
        /// <summary>
        ///     DB Connection
        /// </summary>
        IDbConnection Connection { get; set; }

        /// <summary>
        ///     Order info (Asc,desc, cols)
        /// </summary>
        FilterData FilterData { get; }

        /// <summary>
        ///     SQL Generator
        /// </summary>
        ISqlGenerator<TEntity> SqlGenerator { get; }

        /// <summary>
        ///     Get all objects with orderBy
        /// </summary>
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, string, string>> orderBy);

        /// <summary>
        /// Set columns to select in specified table (model)
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetSelect<T>(Expression<Func<T, object>> expr);

        /// <summary>
        /// Set columns to select in specified table (model)
        /// <param name="expr">The columns to use in order</param>
        /// <param name="permanent">If true, then will be used in all queries</param>
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetSelect<T>(Expression<Func<T, object>> expr, bool permanent);

        /// <summary>
        /// Set custom select expression string
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetSelect(params string[] customSelect);

        /// <summary>
        /// Set custom select expression
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetSelect(Expression<Func<TEntity, object>> expr);

        /// <summary>
        /// Remove query sorting
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetOrderBy();

        /// <summary>
        /// Set columns name (manually) and sorting order (using string)
        /// </summary>
        /// <param name="direction">The sort direction (asc;desc)</param>
        /// <param name="cols">cols name, can be an sql comamnd too.</param>
        IReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, params string[] cols);

        /// <summary>
        /// Set custom query sorting (using string)
        /// </summary>
        /// <param name="query">Your query, must not start with ORDER BY</param>
        IReadOnlyDapperRepository<TEntity> SetOrderBy(string query);

        /// <summary>
        /// Set custom query sorting (using string)
        /// </summary>
        /// <param name="query">Your query, must not start with ORDER BY</param>
        /// <param name="permanent">If you want it to all query in this repository, set to true.</param>
        IReadOnlyDapperRepository<TEntity> SetOrderBy(string query, bool permanent);

        /// <summary>
        /// Set query sorting
        /// <param name="direction">The sort direction (asc;desc)</param>
        /// <param name="permanent">If true, then will be used in all queries</param>
        /// <param name="expr">The columns to use in order</param>
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, bool permanent, Expression<Func<TEntity, object>> expr);

        /// <summary>
        /// Set query sorting
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, Expression<Func<TEntity, object>> expr);

        /// <summary>
        /// Set query sorting for another model (use this when need to sort joined table)
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetOrderBy<T>(OrderInfo.SortDirection direction, Expression<Func<T, object>> expr);

        /// <summary>
        /// Set query sorting for another model (use this when need to sort joined table)
        /// <param name="direction">The sort direction (asc;desc)</param>
        /// <param name="permanent">If true, then will be used in all query</param>
        /// <param name="expr">The columns to use in order</param>
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetOrderBy<T>(OrderInfo.SortDirection direction, bool permanent,
            Expression<Func<T, object>> expr);

        /// <summary>
        /// Remove query grouping
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetGroupBy();


        /// <summary>
        /// Set query grouping
        /// <param name="permanent">If true, then will be used in all queries</param>
        /// <param name="expr">The columns to use in group</param>
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetGroupBy(bool permanent,
            Expression<Func<TEntity, object>> expr);

        /// <summary>
        /// Set query group by using another model (use this when need to group by joined table)
        /// <param name="permanent">If true, then will be used in all queries</param>
        /// <param name="expr">The columns to use in group</param>
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetGroupBy<T>(bool permanent,
            Expression<Func<T, object>> expr);

        /// <summary>
        /// Set query group by using another model (use this when need to group by joined table)
        /// <param name="expr">The columns to use in group</param>
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetOrderBy<T>(Expression<Func<T, object>> expr);


        /// <summary>
        /// Set query grouping
        /// <param name="expr">The columns to use in group</param>
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetGroupBy(Expression<Func<TEntity, object>> expr);

        /// <summary>
        /// Remove limit and offset
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetLimit();

        /// <summary>
        /// Set query offset and limit
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetLimit(uint limit, uint? offset, bool permanent);

        /// <summary>
        /// Set query offset and limit
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetLimit(uint limit, uint offset);

        /// <summary>
        /// Set query limit
        /// </summary>
        IReadOnlyDapperRepository<TEntity> SetLimit(uint limit);
    }
}
