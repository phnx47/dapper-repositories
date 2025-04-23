using MicroOrm.Dapper.Repositories.DbContext;
using Repositories.Base;
using Xunit;

namespace Repositories.MySql.Tests;

public class MySqlConnectorDbContextTests(MySqlConnectorDatabaseFixture fixture) : BaseDapperDbContextTests, IClassFixture<MySqlConnectorDatabaseFixture>
{
    private readonly IDapperDbContext _context = fixture.Db;

    protected override IDapperDbContext CreateContext()
    {
        return _context;
    }
}
