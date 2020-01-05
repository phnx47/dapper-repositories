using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
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
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect<T>(Expression<Func<T, object>> expr)
        {
            if (FilterData.SelectInfo == null)
            {
                FilterData.SelectInfo = new SelectInfo();
            }

            if (FilterData.SelectInfo.Columns == null)
                FilterData.SelectInfo.Columns = new List<string>();

            var type = typeof(T);
            if (expr.NodeType == ExpressionType.Lambda)
            {
                var lambdaUnary = expr.Body as UnaryExpression;
                var expression = lambdaUnary.Operand as MemberExpression;
                var prop = GetProperty(expression, type);
                FilterData.SelectInfo.Columns.Add(prop);
            }
            else
            {
                var cols = (expr.Body as NewExpression)?.Arguments;
                foreach (var expression in cols)
                {
                    var prop = GetProperty(expression, type);
                    if (string.IsNullOrEmpty(prop))
                        continue;
                    
                    FilterData.SelectInfo.Columns.Add(prop);
                }
            }

            return this;
        }


        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect(params string[] customSelect)
        {
            if (FilterData.SelectInfo == null)
            {
                FilterData.SelectInfo = new SelectInfo();
            }

            if (FilterData.SelectInfo.Columns == null)
                FilterData.SelectInfo.Columns = new List<string>();

            FilterData.SelectInfo.Columns = customSelect.ToList();

            return this;
        }


        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect(Expression<Func<TEntity, object>> expr)
        {
            return SetSelect<TEntity>(expr);
        }
    }
}
