using MicroOrm.Dapper.Repositories.DbContext;
using Repositories.Base;
using Xunit;

namespace Repositories.Oracle.Tests;

public class DbContextTests(DatabaseFixture fixture) : BaseDapperDbContextTests, IClassFixture<DatabaseFixture>
{
    private readonly IDapperDbContext _context = fixture.Db;

    protected override IDapperDbContext CreateContext()
    {
        return _context;
    }
}
