using System.Data;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories
{
    /// <summary>
    ///     Base ReadOnlyRepository
    /// </summary>
    public partial class ReadOnlyDapperRepository<TEntity> : IReadOnlyDapperRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ReadOnlyDapperRepository(IDbConnection connection)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(SqlProvider.MSSQL);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReadOnlyDapperRepository(IDbConnection connection, SqlProvider sqlProvider)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(sqlProvider);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReadOnlyDapperRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
        {
            Connection = connection;
            SqlGenerator = sqlGenerator;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReadOnlyDapperRepository(IDbConnection connection, SqlGeneratorConfig config)
        {
            Connection = connection;
            SqlGenerator = new SqlGenerator<TEntity>(config);
        }

        /// <inheritdoc />
        public IDbConnection Connection { get; }

        /// <inheritdoc />
        public ISqlGenerator<TEntity> SqlGenerator { get; }
        
    }
}
