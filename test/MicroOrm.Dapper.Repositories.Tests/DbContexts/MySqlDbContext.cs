using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MySql.Data.MySqlClient;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class MySqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<Car> _cars;

        private IDapperRepository<User> _users;
        private readonly SqlGeneratorConfig _config = new SqlGeneratorConfig
        {
            SqlConnector = ESqlConnector.MySQL,
            UseQuotationMarks = true
        };

        public MySqlDbContext(string connectionString)
            : base(new MySqlConnection(connectionString))
        {
        }

        public IDapperRepository<User> Users => _users ?? (_users = new DapperRepository<User>(Connection, _config));
        public IDapperRepository<Car> Cars => _cars ?? (_cars = new DapperRepository<Car>(Connection, _config));
    }
}