using MicroOrm.Dapper.Repositories.DbContext;
using Repositories.Base;
using Xunit;

namespace Repositories.SQLite.Tests;

public class MicrosoftDbContextTests(MicrosoftDatabaseFixture fixture) : BaseDbContextTests, IClassFixture<MicrosoftDatabaseFixture>
{
    private readonly IDapperDbContext _context = fixture.Db;

    protected override IDapperDbContext CreateContext()
    {
        return _context;
    }
}
