using MicroOrm.Dapper.Repositories.DbContext;
using Repositories.Base;
using Xunit;

namespace Repositories.PostgreSQL.Tests;

public class DbContextTests(DatabaseFixture fixture) : BaseDapperDbContextTests, IClassFixture<DatabaseFixture>
{
    private readonly IDapperDbContext _context = fixture.Db;

    protected override IDapperDbContext CreateContext()
    {
        return _context;
    }
}
