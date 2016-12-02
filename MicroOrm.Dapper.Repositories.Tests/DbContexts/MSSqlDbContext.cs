using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class MSSqlDbContext : DapperDbContext, IDbContext
    {
        public MSSqlDbContext(string connectionString)
           : base(new SqlConnection(connectionString))
        {
        }

        private IDapperRepository<User> _users;
        public IDapperRepository<User> Users => _users ?? (_users = new DapperRepository<User>(Connection, ESqlConnector.MSSQL));

        private IDapperRepository<Car> _cars;
        public IDapperRepository<Car> Cars => _cars ?? (_cars = new DapperRepository<Car>(Connection, ESqlConnector.MSSQL));
    }
}