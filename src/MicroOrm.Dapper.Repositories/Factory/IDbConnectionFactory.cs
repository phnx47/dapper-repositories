using System.Data;

namespace MicroOrm.Dapper.Repositories.Factory
{
    /// <summary>
    /// Connection Factory interface
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// The db conn string
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Create and open the db conn 
        /// </summary>
        IDbConnection OpenDbConnection();

        /// <summary>
        /// Create the db connection
        /// </summary>
        IDbConnection CreateDbConnection();
    }
}
