using MicroOrm.Dapper.Repositories.Factory;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

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
        public ReadOnlyDapperRepository(IDbConnectionFactory factory)
        {
            Factory = factory;
            SqlGenerator = new SqlGenerator<TEntity>();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReadOnlyDapperRepository(IDbConnectionFactory factory, ISqlGenerator<TEntity> sqlGenerator)
        {
            Factory = factory;
            SqlGenerator = sqlGenerator;
        }

        /// <inheritdoc />
        public IDbConnectionFactory Factory { get; }
        
        /// <inheritdoc />
        public FilterData FilterData { get; set; }
        
        /// <inheritdoc />
        public ISqlGenerator<TEntity> SqlGenerator { get; }
        
    }
}
