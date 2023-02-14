using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class SqliteRepositoriesTests : RepositoriesTests, IClassFixture<SqliteDatabaseFixture>
    {
        public SqliteRepositoriesTests(SqliteDatabaseFixture sqliteDatabaseFixture)
            : base(sqliteDatabaseFixture.Db)
        {
        }
    }
}
