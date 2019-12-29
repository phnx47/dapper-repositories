using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            var type = typeof(TEntity);

            var propertyNames = (from s in cols select ExpressionHelper.GetPropertyName(s) into prop select type.GetProperty(prop)?.GetCustomAttribute<ColumnAttribute>() into attr where attr != null select attr.Name).ToList();

            var order = SqlGenerator.FilterData.OrderInfo ?? new OrderInfo();
            order.Direction = direction;
            order.Columns = propertyNames;
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
