using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class MsSqlRepositoriesTests : RepositoriesTests, IClassFixture<MsSqlDatabaseFixture>
    {
        public MsSqlRepositoriesTests(MsSqlDatabaseFixture msSqlDatabaseFixture)
            : base(msSqlDatabaseFixture.Db)
        {
        }
        
        // only Databse specific tests
    }
}