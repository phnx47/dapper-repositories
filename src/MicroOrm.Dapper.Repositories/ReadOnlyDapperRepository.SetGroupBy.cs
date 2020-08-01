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
        public virtual IReadOnlyDapperRepository<TEntity> SetGroupBy()
        {
            FilterData.OrderInfo = null;
            return this;
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetGroupBy(bool permanent,
            Expression<Func<TEntity, object>> expr)
        {
            return SetGroupBy<TEntity>(permanent, expr);
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetGroupBy<T>(bool permanent,
            Expression<Func<T, object>> expr)
        {
            var order = FilterData.GroupInfo ?? new GroupInfo();

            var type = typeof(T);
            if (expr.Body.NodeType == ExpressionType.Convert)
            {
                var lambdaUnary = expr.Body as UnaryExpression;
                var expression = lambdaUnary.Operand as MemberExpression;
                order.Columns = new List<string> {GetProperty(expression, type)};
            } else if (expr.Body.NodeType == ExpressionType.MemberAccess)
            {
                order.Columns = new List<string> { GetProperty(expr.Body, type) };
            }
            else
            {
                var cols = (expr.Body as NewExpression)?.Arguments;
                var propertyNames = cols?.Select(expression => GetProperty(expression, type)).ToList();
                order.Columns = propertyNames;
            }

            order.Permanent = permanent;

            FilterData.GroupInfo = order;

            return this;
        }

        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetGroupBy(Expression<Func<TEntity, object>> expr)
        {
            return SetGroupBy(false, expr);
        }
        
        /// <inheritdoc />
        public virtual IReadOnlyDapperRepository<TEntity> SetOrderBy<T>(Expression<Func<T, object>> expr)
        {
            return SetGroupBy(false, expr);
        }
    }
}
