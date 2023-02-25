using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
#if Second
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
#else
using Microsoft.Data.Sqlite;
#endif

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class SqliteDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<Address> _address;

        private IDapperRepository<Car> _cars;

        private IDapperRepository<User> _users;

        private IDapperRepository<City> _cities;

        private IDapperRepository<Report> _reports;

        private IDapperRepository<Phone> _phones;

        public SqliteDbContext(string connectionString) : base(new SqliteConnection(connectionString))
        {

        }

        public IDapperRepository<Address> Address => _address ?? (_address = new DapperRepository<Address>(Connection, new SqlGenerator<Address>(SqlProvider.SQLite, true)));

        public IDapperRepository<User> Users => _users ?? (_users = new DapperRepository<User>(Connection, new SqlGenerator<User>(SqlProvider.SQLite, true)));

        public IDapperRepository<Car> Cars => _cars ?? (_cars = new DapperRepository<Car>(Connection, new SqlGenerator<Car>(SqlProvider.SQLite, true)));

        public IDapperRepository<City> Cities => _cities ?? (_cities = new DapperRepository<City>(Connection, new SqlGenerator<City>(SqlProvider.SQLite, true)));

        public IDapperRepository<Report> Reports => _reports ?? (_reports = new DapperRepository<Report>(Connection, new SqlGenerator<Report>(SqlProvider.SQLite, true)));

        public IDapperRepository<Phone> Phones => _phones ?? (_phones = new DapperRepository<Phone>(Connection, new SqlGenerator<Phone>(SqlProvider.SQLite, true)));
    }
}
