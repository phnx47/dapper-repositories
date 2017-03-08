using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class MSSqlDbContext : DapperDbContext, IDbContext
    {
        private SqlGeneratorConfig config = new SqlGeneratorConfig()
        {
            SqlConnector = ESqlConnector.MSSQL,
            UseQuotationMarks = true
        };

        private IDapperRepository<Address> _address;

        private IDapperRepository<Car> _cars;

        private IDapperRepository<User> _users;

        public MSSqlDbContext(string connectionString)
            : base(new SqlConnection(connectionString))
        {
        }

        public IDapperRepository<Address> Address => _address ?? (_address = new DapperRepository<Address>(Connection, config));
        public IDapperRepository<User> Users => _users ?? (_users = new DapperRepository<User>(Connection, config));
        public IDapperRepository<Car> Cars => _cars ?? (_cars = new DapperRepository<Car>(Connection, config));
    }
}