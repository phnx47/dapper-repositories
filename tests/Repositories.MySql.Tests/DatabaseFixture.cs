using System;
using Dapper;
using Repositories.Base;

namespace Repositories.MySql.Tests;

public class DatabaseFixture : IDisposable
{
    private const string _dbName = "test_main"; // DisableTestParallelization=true

    protected DatabaseFixture(TestDbContext db)
    {
        Db = db;
        DropDatabase();
        InitDb();
    }

    public TestDbContext Db { get; }

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
                              "`IsActive` boolean not null, `Code` varchar(256) not null, `Deleted` boolean, PRIMARY KEY  (`Id`));");

        Db.Connection.Execute($"USE `{_dbName}`");

        Db.Connection.Execute("CREATE TABLE IF NOT EXISTS `Users` " +
                              "(`Id` int not null auto_increment, `Name` varchar(256) not null, `AddressId` int not null, `PhoneId` int not null, " +
                              "`OfficePhoneId` int not null, `Deleted` boolean, `UpdatedAt` datetime, PRIMARY KEY  (`Id`));");

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
        Db.Connection.Execute("DROP DATABASE IF EXISTS DAB");
    }
}
