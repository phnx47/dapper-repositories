using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class OracleRepositoriesTests : RepositoriesTests, IClassFixture<OracleDatabaseFixture>
    {
        public OracleRepositoriesTests(OracleDatabaseFixture oracleDatabaseFixture)
            : base(oracleDatabaseFixture.Db)
        {
        }
    }
}
