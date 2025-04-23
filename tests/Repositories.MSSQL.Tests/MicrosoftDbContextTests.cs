using MicroOrm.Dapper.Repositories.DbContext;
using Repositories.Base;
using Xunit;

namespace Repositories.MSSQL.Tests;

public class MicrosoftDbContextTests(MicrosoftDatabaseFixture fixture) : BaseDapperDbContextTests, IClassFixture<MicrosoftDatabaseFixture>
{
    private readonly IDapperDbContext _context = fixture.Db;

    protected override IDapperDbContext CreateContext()
    {
        return _context;
    }
}
