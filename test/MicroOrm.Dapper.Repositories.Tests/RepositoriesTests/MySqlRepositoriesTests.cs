using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class MySqlRepositoriesTests : RepositoriesTests, IClassFixture<MySqlDatabaseFixture>
    {
        public MySqlRepositoriesTests(MySqlDatabaseFixture mySqlDatabaseFixture)
            : base(mySqlDatabaseFixture.Db)
        {
        }
    }
}