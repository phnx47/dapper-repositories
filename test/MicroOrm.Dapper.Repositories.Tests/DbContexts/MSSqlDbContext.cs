using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class MsSqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<Address> _address;

        private IDapperRepository<Car> _cars;

        private IDapperRepository<User> _users;

        private IDapperRepository<City> _cities;

        private IDapperRepository<Report> _reports;

        private IDapperRepository<Phone> _phones;

        private readonly SqlGeneratorConfig _config = new SqlGeneratorConfig
        {
            SqlProvider = SqlProvider.MSSQL,
            UseQuotationMarks = true
        };

        public MsSqlDbContext(string connectionString)
            : base(new SqlConnection(connectionString))
        {
        }

        public IDapperRepository<Address> Address => _address ?? (_address = new DapperRepository<Address>(Connection, _config));
        public IDapperRepository<User> Users => _users ?? (_users = new DapperRepository<User>(Connection, _config));
        public IDapperRepository<Car> Cars => _cars ?? (_cars = new DapperRepository<Car>(Connection, _config));
        public IDapperRepository<City> Cities => _cities ?? (_cities = new DapperRepository<City>(Connection, _config));
        public IDapperRepository<Report> Reports => _reports ?? (_reports = new DapperRepository<Report>(Connection, _config));
        public IDapperRepository<Phone> Phones => _phones ?? (_phones = new DapperRepository<Phone>(Connection, _config));
    }
}