using System.Data;
using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using TestClasses;

namespace Repositories.Base;

public class TestDbContext(IDbConnection connection, SqlProvider provider, bool useQuotationMarks = false) : DapperDbContext(connection)
{
    private IDapperRepository<Address> _address;

    private IDapperRepository<Car> _cars;

    private IDapperRepository<User> _users;

    private IDapperRepository<City> _cities;

    private IDapperRepository<Report> _reports;

    private IDapperRepository<Phone> _phones;

    public IDapperRepository<Address> Address => _address ??= new DapperRepository<Address>(Connection, new SqlGenerator<Address>(provider, useQuotationMarks));

    public IDapperRepository<User> Users => _users ??= new DapperRepository<User>(Connection, new SqlGenerator<User>(provider, useQuotationMarks));

    public IDapperRepository<Car> Cars => _cars ??= new DapperRepository<Car>(Connection, new SqlGenerator<Car>(provider, useQuotationMarks));

    public IDapperRepository<City> Cities => _cities ??= new DapperRepository<City>(Connection, new SqlGenerator<City>(provider, useQuotationMarks));

    public IDapperRepository<Report> Reports => _reports ??= new DapperRepository<Report>(Connection, new SqlGenerator<Report>(provider, useQuotationMarks));

    public IDapperRepository<Phone> Phones => _phones ??= new DapperRepository<Phone>(Connection, new SqlGenerator<Phone>(provider, useQuotationMarks));
}
