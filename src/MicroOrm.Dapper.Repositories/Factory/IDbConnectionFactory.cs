using System.Data;

namespace MicroOrm.Dapper.Repositories.Factory
{
    public interface IDbConnectionFactory
    {
        string ConnectionString { get; }
        IDbConnection OpenDbConnection();
        IDbConnection CreateDbConnection();
    }
}
