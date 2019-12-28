using System;
using System.Linq;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetOrderBy()
        {
            SqlGenerator.FilterData.OrderInfo = null;
            return this;
        }

        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, bool permanent,
            params Expression<Func<TEntity, object>>[] cols)
        {
            var arr = cols.Select(col => ExpressionHelper.GetPropertyName(col)).ToList();
            var order = SqlGenerator.FilterData.OrderInfo ?? new OrderInfo();
            order.Direction = direction;
            order.Columns = arr;
            order.Permanent = permanent;
            
            SqlGenerator.FilterData.OrderInfo = order;
            
            return this;
        }

        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, params Expression<Func<TEntity, object>>[] cols)
        {
            return SetOrderBy(direction, false, cols);
        }
    }
}
