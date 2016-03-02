using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories.Tests.Classes;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class MSSqlDbContext : DbContext
    {
        public MSSqlDbContext(string connectionString)
           : base(new SqlConnection(connectionString))
        {
        }

        private IDapperRepository<User> _users;
        public IDapperRepository<User> Users => _users ?? (_users = new DapperRepository<User>(Connection));

        private IDapperRepository<Car> _cars;
        public IDapperRepository<Car> Cars => _cars ?? (_cars = new DapperRepository<Car>(Connection));
    }
}