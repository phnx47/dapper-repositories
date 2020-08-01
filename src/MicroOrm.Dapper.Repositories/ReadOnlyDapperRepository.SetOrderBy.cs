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
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy()
        {
            FilterData.OrderInfo = null;
            return this;
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, params string[] cols)
        {
            var order = FilterData.OrderInfo ?? new OrderInfo();

            order.Direction = direction;
            order.Columns = cols.ToList();

            FilterData.OrderInfo = order;

            return this;
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy(string query)
        {
            return SetOrderBy(query, false);
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy(string query, bool permanent)
        {
            var order = FilterData.OrderInfo ?? new OrderInfo();

            order.Direction = null;
            order.CustomQuery = query;
            order.Permanent = permanent;
            FilterData.OrderInfo = order;

            return this;
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, bool permanent,
            Expression<Func<TEntity, object>> expr)
        {
            return SetOrderBy<TEntity>(direction, permanent, expr);
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy<T>(OrderInfo.SortDirection direction, bool permanent,
            Expression<Func<T, object>> expr)
        {
            var order = FilterData.OrderInfo ?? new OrderInfo();
            order.Direction = direction;

            var type = typeof(T);
            if (expr.Body.NodeType == ExpressionType.Convert)
            {
                var lambdaUnary = expr.Body as UnaryExpression;
                var expression = lambdaUnary.Operand as MemberExpression;
                order.Columns = new List<string> {GetProperty(expression, type)};
            }
            else if (expr.Body.NodeType == ExpressionType.MemberAccess)
            {
                order.Columns = new List<string> {GetProperty(expr.Body, type)};
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
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy(OrderInfo.SortDirection direction, Expression<Func<TEntity, object>> expr)
        {
            return SetOrderBy(direction, false, expr);
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy<T>(OrderInfo.SortDirection direction, Expression<Func<T, object>> expr)
        {
            return SetOrderBy(direction, false, expr);
        }
    }
}
