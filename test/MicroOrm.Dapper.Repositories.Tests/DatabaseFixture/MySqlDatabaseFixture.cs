using System;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MySqlDatabaseFixture : IDisposable
    {
        private const string _dbName = "test_micro_orm";

        public MySqlDatabaseFixture()
        {
            var connString = $"Database={_dbName};Server=localhost;Uid=root;Pwd=root;";

            Db = new MySqlDbContext(connString);
        }

        public MySqlDbContext Db { get; }

        public void Dispose()
        {
        }

        private void InitDb()
        {
        }
    }
}