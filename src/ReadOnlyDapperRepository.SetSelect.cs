using System;
using System.Linq;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace MicroOrm.Dapper.Repositories;

/// <summary>
///     Base Repository
/// </summary>
public partial class ReadOnlyDapperRepository<TEntity>
    where TEntity : class
{

    public virtual IReadOnlyDapperRepository<TEntity> SetSelect<T>(Expression<Func<T, object>> expr, bool permanent)
    {
        if (FilterData.SelectInfo == null)
        {
            FilterData.SelectInfo = new SelectInfo();
        }

        FilterData.SelectInfo.Permanent = permanent;

        var type = typeof(T);
        if (expr.Body.NodeType == ExpressionType.Lambda)
        {
            if (expr.Body is UnaryExpression { Operand: MemberExpression expression })
            {
                var prop = GetProperty(expression, type);
                FilterData.SelectInfo.Columns.Add(prop);
            }
        }
        else
        {
            var cols = (expr.Body as NewExpression)?.Arguments;
            if (cols != null)
            {
                foreach (var expression in cols)
                {
                    var prop = GetProperty(expression, type);
                    if (string.IsNullOrEmpty(prop))
                        continue;

                    FilterData.SelectInfo.Columns.Add(prop);
                }
            }
        }

        return this;
    }


    public virtual IReadOnlyDapperRepository<TEntity> SetSelect(params string[] customSelect)
    {
        if (FilterData.SelectInfo == null)
        {
            FilterData.SelectInfo = new SelectInfo();
        }

        FilterData.SelectInfo.Columns = customSelect.ToList();

        return this;
    }


    public virtual IReadOnlyDapperRepository<TEntity> SetSelect(Expression<Func<TEntity, object>> expr)
    {
        return SetSelect(expr, false);
    }


    public virtual IReadOnlyDapperRepository<TEntity> SetSelect<T>(Expression<Func<T, object>> expr)
    {
        return SetSelect(expr, false);
    }
}
