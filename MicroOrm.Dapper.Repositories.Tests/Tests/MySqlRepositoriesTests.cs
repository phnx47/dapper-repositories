using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;


namespace MicroOrm.Dapper.Repositories.Tests.Tests
{
    public class MySqlRepositoriesTests : IClassFixture<MsSqlDatabaseFixture>
    {
        private readonly MsSqlDatabaseFixture _sqlDatabaseFixture;

        public MySqlRepositoriesTests(MsSqlDatabaseFixture msSqlDatabaseFixture)
        {
            _sqlDatabaseFixture = msSqlDatabaseFixture;
        }

    }
}