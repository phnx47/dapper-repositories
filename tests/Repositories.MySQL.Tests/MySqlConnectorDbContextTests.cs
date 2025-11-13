using MicroOrm.Dapper.Repositories.DbContext;
using Repositories.Base;
using Xunit;

namespace Repositories.MySQL.Tests;

public class MySqlConnectorDbContextTests(MySqlConnectorDatabaseFixture fixture) : BaseDbContextTests, IClassFixture<MySqlConnectorDatabaseFixture>
{
    private readonly IDapperDbContext _context = fixture.Db;

    protected override IDapperDbContext CreateContext()
    {
        return _context;
    }
}
