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
            const string connString = "Server=localhost;Uid=root;Pwd=Password12!;SslMode=none";
            
            Db = new MySqlDbContext(connString);

            InitDb();
        }

        public MySqlDbContext Db { get; }

        public void Dispose()
        {
            Db.Connection.Execute($"DROP DATABASE {_dbName}");
            Db.Dispose();
        }

        private void InitDb()
        {
            Db.Connection.Execute($"CREATE DATABASE IF NOT EXISTS `{_dbName}`;");
            Db.Connection.Execute($"USE `{_dbName}`");

            ClearDb();

            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Users` (`Id` int(11) NOT NULL auto_increment, `Name` varchar(256) NOT NULL, PRIMARY KEY  (`Id`));");
            var t = 2;
        }
        
        private void ClearDb()
        {    
            void DropTable(string name)
            {
                Db.Connection.Execute($"DROP TABLE IF EXISTS {name};");
            }

            DropTable("Users");
            /*DropTable("dbo", "Cars");
            DropTable("dbo", "Addresses");
            DropTable("dbo", "Cities");
            DropTable("dbo", "Reports");
            DropTable("DAB", "Phones");*/
        }
    }
}
