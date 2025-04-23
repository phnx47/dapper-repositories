using MicroOrm.Dapper.Repositories.DbContext;
using Repositories.Base;
using Xunit;

namespace Repositories.MySql.Tests;

public class MySqlDbContextTests(MySqlClientDatabaseFixture fixture) : BaseDbContextTests, IClassFixture<MySqlClientDatabaseFixture>
{
    private readonly IDapperDbContext _context = fixture.Db;

    protected override IDapperDbContext CreateContext()
    {
        return _context;
    }
}
