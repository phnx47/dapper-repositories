using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MySqlDatabaseFixture : IDisposable
    {
        /*
         * docker run --rm --name=mysql -d -p 3306:3306 -e MYSQL_ROOT_PASSWORD=Password12! mysql
         */
        
        private const string _dbName = "test_micro_orm";

        
        public MySqlDatabaseFixture()
        {
            string connString = "Server=localhost;Uid=root;Pwd=";

            if (Environments.IsAppVeyor)
                connString = "Server=localhost;Uid=root;Pwd=Password12!";

            Db = new MySqlDbContext(connString);

            InitDb();
        }

        public MySqlDbContext Db { get; }

        public void Dispose()
        {
            Db.Connection.Execute($"DROP DATABASE {_dbName}");
            Db.Connection.Execute($"DROP DATABASE DAB");
            Db.Dispose();
        }

        private void InitDb()
        {
            Db.Connection.Execute($"CREATE DATABASE IF NOT EXISTS `{_dbName}`;");
            Db.Connection.Execute($"CREATE DATABASE IF NOT EXISTS DAB;");
            
            Db.Connection.Execute($"USE `{_dbName}`");

            ClearDb();

            Db.Connection.Execute($"USE `DAB`");
            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Phones` " + 
                "(`Id` int not null auto_increment, `Number` varchar(256) not null, "  +
                "`IsActive` boolean not null, `Code` varchar(256) not null, PRIMARY KEY  (`Id`));");
            
            Db.Connection.Execute($"USE `{_dbName}`");
            
            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Users` " + 
                "(`Id` int not null auto_increment, `Name` varchar(256) not null, `AddressId` int not null, `PhoneId` int not null, "  +
                "`OfficePhoneId` int not null, `Deleted` boolean not null, `UpdatedAt` datetime, PRIMARY KEY  (`Id`));");
            
            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Cars` " + 
                "(`Id` int not null auto_increment, `Name` varchar(256) not null, "  +
                "`UserId` int not null, `Status` int not null, Data binary(16) null, PRIMARY KEY  (`Id`));");
            
            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Addresses`" + 
                "(`Id` int not null auto_increment, `Street` varchar(256) not null, "  +
                "`CityId` varchar(256) not null, PRIMARY KEY  (`Id`));");
            
            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Cities`" + 
                "(`Id` int not null auto_increment, `Name` varchar(256) not null, `Identifier` char(36) not null, "  +
                "PRIMARY KEY  (`Id`));");
            
            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Reports`" + 
                "(`Id` int not null auto_increment, `AnotherId` int not null, `UserId` int not null, "  +
                "PRIMARY KEY  (`Id`, `AnotherId`));");
            
            
            InitData.Execute(Db);
        }
        
        private void ClearDb()
        {    
            void DropTable(string name)
            {
                Db.Connection.Execute($"DROP TABLE IF EXISTS {name};");
            }
            
            Db.Connection.Execute($"USE `{_dbName}`");
            DropTable("Users");
            DropTable("Addresses");
            DropTable("Cities");
            DropTable("Reports");
            DropTable("Cars");
            
            Db.Connection.Execute($"USE `DAB`");
            DropTable("Phones");
        }
    }
}
