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
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect<TChild>(params Expression<Func<TEntity, object>>[] cols)
        {
            return this;
        }

        /// <inheritdoc />
        public virtual ReadOnlyDapperRepository<TEntity> SetSelect(params Expression<Func<TEntity, object>>[] cols)
        {
            return this;
        }
    }
}
