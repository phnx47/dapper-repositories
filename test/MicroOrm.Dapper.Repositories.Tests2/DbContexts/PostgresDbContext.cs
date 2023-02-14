using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Npgsql;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class PostgresDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<Address> _address;

        private IDapperRepository<Car> _cars;

        private IDapperRepository<User> _users;

        private IDapperRepository<City> _cities;

        private IDapperRepository<Report> _reports;

        private IDapperRepository<Phone> _phones;

        public PostgresDbContext(string connectionString)
            : base(new NpgsqlConnection(connectionString))
        {
        }

        public void SetConnectionString(string connectionString)
        {
            InnerConnection.Close();

            InnerConnection.ConnectionString = connectionString;
        }

        public IDapperRepository<Address> Address => _address ?? (_address = new DapperRepository<Address>(Connection, new SqlGenerator<Address>(SqlProvider.PostgreSQL)));

        public IDapperRepository<User> Users => _users ?? (_users = new DapperRepository<User>(Connection, new SqlGenerator<User>(SqlProvider.PostgreSQL)));

        public IDapperRepository<Car> Cars => _cars ?? (_cars = new DapperRepository<Car>(Connection, new SqlGenerator<Car>(SqlProvider.PostgreSQL)));

        public IDapperRepository<City> Cities => _cities ?? (_cities = new DapperRepository<City>(Connection, new SqlGenerator<City>(SqlProvider.PostgreSQL)));

        public IDapperRepository<Report> Reports => _reports ?? (_reports = new DapperRepository<Report>(Connection, new SqlGenerator<Report>(SqlProvider.PostgreSQL)));

        public IDapperRepository<Phone> Phones => _phones ?? (_phones = new DapperRepository<Phone>(Connection, new SqlGenerator<Phone>(SqlProvider.PostgreSQL)));
    }
}
