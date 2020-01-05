using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            FilterData.OrderInfo = null;
            return this;
        }

        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, bool permanent,
            Expression<Func<TEntity, object>> expr)
        {
            return SetOrderBy<TEntity>(direction, permanent, expr);
        }

        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetOrderBy<T>(OrderInfo.SortDirection direction, bool permanent,
            Expression<Func<T, object>> expr)
        {
            var order = FilterData.OrderInfo ?? new OrderInfo();
            order.Direction = direction;

            var type = typeof(T);
            if (expr.NodeType == ExpressionType.Lambda)
            {
                var lambdaUnary = expr.Body as UnaryExpression;
                var expression = lambdaUnary.Operand as MemberExpression;
                order.Columns = new List<string> {GetProperty(expression, type)};
            }
            else
            {
                var cols = (expr.Body as NewExpression)?.Arguments;
                var propertyNames = cols.Select(expression => GetProperty(expression, type)).ToList();
                order.Columns = propertyNames;
            }

            order.Permanent = permanent;

            FilterData.OrderInfo = order;

            return this;
        }

        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, Expression<Func<TEntity, object>> expr)
        {
            return SetOrderBy(direction, false, expr);
        }
    }
}
