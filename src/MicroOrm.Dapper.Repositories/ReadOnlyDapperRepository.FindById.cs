using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class ReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public virtual TEntity FindById(object id)
        {
            return FindById(id, null);
        }

        /// <inheritdoc />
        public virtual TEntity FindById(object id, IDbTransaction transaction)
        {
            var queryResult = SqlGenerator.GetSelectById(id, null);
            return Connection.QuerySingleOrDefault<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }

        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync(object id)
        {
            return FindByIdAsync(id, null);
        }

        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync(object id, IDbTransaction transaction)
        {
            var queryResult = SqlGenerator.GetSelectById(id, null);
            return Connection.QuerySingleOrDefaultAsync<TEntity>(queryResult.GetSql(), queryResult.Param, transaction);
        }
    }
}
