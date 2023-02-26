using System.Data;
using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using TestClasses;

namespace Repositories.Base;

public class TestDbContext : DapperDbContext
{
    private IDapperRepository<Address> _address;

    private IDapperRepository<Car> _cars;

    private IDapperRepository<User> _users;

    private IDapperRepository<City> _cities;

    private IDapperRepository<Report> _reports;

    private IDapperRepository<Phone> _phones;

    private readonly SqlProvider _provider;
    private readonly bool _useQuotationMarks;

    public TestDbContext(IDbConnection connection, SqlProvider provider, bool useQuotationMarks = false)
        : base(connection)
    {
        _provider = provider;
        _useQuotationMarks = useQuotationMarks;
    }

    public IDapperRepository<Address> Address => _address ??= new DapperRepository<Address>(Connection, new SqlGenerator<Address>(_provider, _useQuotationMarks));

    public IDapperRepository<User> Users => _users ??= new DapperRepository<User>(Connection, new SqlGenerator<User>(_provider, _useQuotationMarks));

    public IDapperRepository<Car> Cars => _cars ??= new DapperRepository<Car>(Connection, new SqlGenerator<Car>(_provider, _useQuotationMarks));

    public IDapperRepository<City> Cities => _cities ??= new DapperRepository<City>(Connection, new SqlGenerator<City>(_provider, _useQuotationMarks));

    public IDapperRepository<Report> Reports => _reports ??= new DapperRepository<Report>(Connection, new SqlGenerator<Report>(_provider, _useQuotationMarks));

    public IDapperRepository<Phone> Phones => _phones ??= new DapperRepository<Phone>(Connection, new SqlGenerator<Phone>(_provider, _useQuotationMarks));
}
