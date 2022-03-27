using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;
using Xunit.Abstractions;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class Oracle12cRepositoriesTests : RepositoriesTests, IClassFixture<Oracle12cDatabaseFixture>
    {
        public Oracle12cRepositoriesTests(Oracle12cDatabaseFixture Oracle12cDatabaseFixture, ITestOutputHelper testOutputHelper)
            : base(Oracle12cDatabaseFixture.Db, testOutputHelper)
        {
        }
    }
}
