using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MySql.Data.MySqlClient;

namespace MicroOrm.Dapper.Repositories.Tests.DbContexts
{
    public class MySqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<Address> _address;

        private IDapperRepository<Car> _cars;

        private IDapperRepository<User> _users;

        private IDapperRepository<City> _cities;

        private IDapperRepository<Report> _reports;

        private IDapperRepository<Phone> _phones;

        private readonly SqlGeneratorConfig _config = new SqlGeneratorConfig
        {
            SqlProvider = SqlProvider.MySQL,
            UseQuotationMarks = true
        };

        public MySqlDbContext(string connectionString)
            : base(new MySqlConnection(connectionString))
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