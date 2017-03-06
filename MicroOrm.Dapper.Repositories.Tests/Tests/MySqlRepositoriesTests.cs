using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.Tests
{
    public class MySqlRepositoriesTests : IClassFixture<MySqlDatabaseFixture>
    {
        private readonly MySqlDatabaseFixture _sqlDatabaseFixture;

        public MySqlRepositoriesTests(MySqlDatabaseFixture msSqlDatabaseFixture)
        {
            _sqlDatabaseFixture = msSqlDatabaseFixture;
        }
    }
}