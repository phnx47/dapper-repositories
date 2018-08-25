using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MySqlDatabaseFixture : IDisposable
    {
        private const string _dbName = "test_micro_orm";


        public MySqlDatabaseFixture()
        {
            var connString = "Server=localhost;Uid=user_test_micro_orm;Pwd=Password12!;SslMode=none";

            if (Environments.IsAppVeyor)
                connString = "Server=localhost;Uid=root;Pwd=Password12!;SslMode=none";

            Db = new MySqlDbContext(connString);

            InitDb();
        }

        public MySqlDbContext Db { get; }

        public void Dispose()
        {
        }

        private void InitDb()
        {
            Db.Connection.Execute($"CREATE DATABASE IF NOT EXISTS `{_dbName}`;");
            Db.Connection.Execute($"USE `{_dbName}`");


            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Users` (`Id` int(11) NOT NULL auto_increment, PRIMARY KEY  (`Id`));");
        }
    }
}