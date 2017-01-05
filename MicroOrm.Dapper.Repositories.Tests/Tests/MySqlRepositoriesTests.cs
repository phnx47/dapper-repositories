using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;


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