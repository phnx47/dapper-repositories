using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect<TChild>(Expression<Func<TEntity, object>>[] cols)
        {
            return this;
        }

        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect(params Expression<Func<TEntity, object>>[] cols)
        {
            Console.WriteLine(cols);
            
            if (FilterData.SelectInfo.Columns == null)
            {
                FilterData.SelectInfo.Columns = new List<string>();
            }
            
            foreach (var fieldName in cols.Select(ExpressionHelper.GetPropertyName)) 
                FilterData.SelectInfo.Columns.Add(SqlGenerator.SqlProperties.First(x => x.PropertyName == fieldName).ColumnName);

            return this;
        }
    }
}
