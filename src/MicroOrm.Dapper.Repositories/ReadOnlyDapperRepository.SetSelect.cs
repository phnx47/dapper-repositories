using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MicroOrm.Dapper.Repositories.Extensions;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect<TChild>(Expression<Func<TChild, object>> expr)
        {
            if (FilterData.SelectInfo == null)
            {
                FilterData.SelectInfo = new SelectInfo();
            }

            if (FilterData.SelectInfo.Columns == null)
                FilterData.SelectInfo.Columns = new Dictionary<string, SqlPropertyMetadata>();

            var type = typeof(TChild);
            
            var cols = (expr.Body as NewExpression)?.Arguments;
            foreach (var expression in cols)
            {
                var field = (MemberExpression) expression;
                
                var prop = type.GetProperty(field.Member.Name);
                
                var declaringType = type.GetTypeInfo();
                var tableAttribute = declaringType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttribute != null ? tableAttribute.Name : declaringType.Name;
                
                if (prop.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;
                
                FilterData.SelectInfo.Columns.Add(tableName, new SqlPropertyMetadata(prop));
            }

            return this;
        }


        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect(Expression<Func<TEntity, object>> expr)
        {
            if (FilterData.SelectInfo == null)
            {
                FilterData.SelectInfo = new SelectInfo();
            }

            if (FilterData.SelectInfo.Columns == null)
                FilterData.SelectInfo.Columns = new Dictionary<string, SqlPropertyMetadata>();

            var type = typeof(TEntity);
            
            var cols = (expr.Body as NewExpression)?.Arguments;
            foreach (var expression in cols)
            {
                var field = (MemberExpression) expression;
                
                var prop = type.GetProperty(field.Member.Name);
                
                var declaringType = type.GetTypeInfo();
                var tableAttribute = declaringType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttribute != null ? tableAttribute.Name : declaringType.Name;
                
                if (prop.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;
                
                FilterData.SelectInfo.Columns.Add(tableName, new SqlPropertyMetadata(prop));
            }

            return this;
        }
    }
}
