using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;
using Xunit.Abstractions;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class MsSqlRepositoriesTests : RepositoriesTests, IClassFixture<MsSqlDatabaseFixture>
    {
        public MsSqlRepositoriesTests(MsSqlDatabaseFixture msSqlDatabaseFixture, ITestOutputHelper testOutputHelper)
            : base(msSqlDatabaseFixture.Db, testOutputHelper)
        {
        }
        
        // only Databse specific tests
    }
}
