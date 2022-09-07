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
            Db = new MySqlDbContext("Server=localhost;Uid=root;Pwd=Password12!");

            DropDatabase();
            InitDb();
        }

        public MySqlDbContext Db { get; }

        public void Dispose()
        {
            DropDatabase();
            Db.Dispose();
        }

        private void InitDb()
        {

            Db.Connection.Execute($"CREATE DATABASE IF NOT EXISTS `{_dbName}`;");
            Db.Connection.Execute($"CREATE DATABASE IF NOT EXISTS DAB;");

            Db.Connection.Execute($"USE `{_dbName}`");


            Db.Connection.Execute($"USE `DAB`");
            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Phones` " +
                                  "(`Id` int not null auto_increment, `PNumber` varchar(256) not null, " +
                                  "`IsActive` boolean not null, `Code` varchar(256) not null, PRIMARY KEY  (`Id`));");

            Db.Connection.Execute($"USE `{_dbName}`");

            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Users` " +
                                  "(`Id` int not null auto_increment, `Name` varchar(256) not null, `AddressId` int not null, `PhoneId` int not null, " +
                                  "`OfficePhoneId` int not null, `Deleted` boolean not null, `UpdatedAt` datetime, PRIMARY KEY  (`Id`));");

            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Cars` " +
                                  "(`Id` int not null auto_increment, `Name` varchar(256) not null, " +
                                  "`UserId` int not null, `Status` int not null, Data binary(16) null, PRIMARY KEY  (`Id`));");

            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Addresses`" +
                                  "(`Id` int not null auto_increment, `Street` varchar(256) not null, " +
                                  "`CityId` varchar(256) not null, PRIMARY KEY  (`Id`));");

            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Cities`" +
                                  "(`Id` int not null auto_increment, `Name` varchar(256) not null, `Identifier` char(36) not null, " +
                                  "PRIMARY KEY  (`Id`));");

            Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Reports`" +
                                  "(`Id` int not null auto_increment, `AnotherId` int not null, `UserId` int not null, " +
                                  "PRIMARY KEY  (`Id`, `AnotherId`));");


            InitData.Execute(Db);
        }

        private void DropDatabase()
        {
            Db.Connection.Execute($"DROP DATABASE IF EXISTS {_dbName}");
            Db.Connection.Execute($"DROP DATABASE IF EXISTS DAB");
        }
    }
}
