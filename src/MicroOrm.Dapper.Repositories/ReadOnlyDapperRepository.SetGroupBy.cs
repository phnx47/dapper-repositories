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
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                {
                    if (expr.Body is UnaryExpression lambdaUnary)
                    {
                        var expression = lambdaUnary.Operand as MemberExpression;
                        order.Columns = new List<string> {GetProperty(expression, type)};
                    }

                    break;
                }
                case ExpressionType.MemberAccess:
                    order.Columns = new List<string> { GetProperty(expr.Body, type) };
                    break;
                default:
                {
                    var cols = (expr.Body as NewExpression)?.Arguments;
                    var propertyNames = cols?.Select(expression => GetProperty(expression, type)).ToList();
                    order.Columns = propertyNames;
                    break;
                }
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
