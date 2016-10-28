using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MySqlDatabaseFixture : IDisposable
    {
        private const string DbName = "test_micro_orm";

        public MySqlDatabaseFixture()
        {
            var connString = $"Database={DbName};Server=(local);Uid=root;Pwd=repytwjd;";

            Db = new MySqlDbContext(connString);
        }

        public void Dispose()
        {

        }

        public MySqlDbContext Db { get; }

        private void InitDb()
        {

        }
    }
}
