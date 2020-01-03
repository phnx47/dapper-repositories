using MicroOrm.Dapper.Repositories.Factory;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base Repository
    /// </summary>
    public partial class DapperRepository<TEntity> : ReadOnlyDapperRepository<TEntity>, IDapperRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public DapperRepository(IDbConnectionFactory connection)
            : base(connection)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public DapperRepository(IDbConnectionFactory connection, ISqlGenerator<TEntity> sqlGenerator)
            : base(connection, sqlGenerator)
        {
        }

    }
}
