using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class SqliteDatabaseFixture : IDisposable
    {
        private const string _dbName = "test_micro_orm";

        public SqliteDatabaseFixture()
        {
            Db = new SqliteDbContext($"Data Source={Guid.NewGuid():N};Mode=Memory;Cache=Shared");

            InitDb();
        }

        public SqliteDbContext Db { get; }

        public void Dispose()
        {
            Db.Dispose();
        }

        private void InitDb()
        {
            Db.Connection.Execute("CREATE TABLE `DAB.Phones` " +
                                  "(`Id` integer not null primary key autoincrement, `PNumber` varchar(256) not null, " +
                                  "`IsActive` boolean not null, `Code` varchar(256) not null);");

            Db.Connection.Execute("CREATE TABLE `Users` " +
                                  "(`Id` integer not null primary key autoincrement, `Name` varchar(256) not null, `AddressId` integer not null, `PhoneId` integer not null, " +
                                  "`OfficePhoneId` integer not null, `Deleted` boolean not null, `UpdatedAt` datetime);");

            Db.Connection.Execute("CREATE TABLE `Cars` " +
                                  "(`Id` integer not null primary key autoincrement, `Name` varchar(256) not null, " +
                                  "`UserId` integer not null, `Status` integer not null, Data binary(16) null);");

            Db.Connection.Execute("CREATE TABLE `Addresses`" +
                                  "(`Id` integer not null primary key autoincrement, `Street` varchar(256) not null, " +
                                  "`CityId` varchar(256) not null);");

            Db.Connection.Execute("CREATE TABLE `Cities`" +
                                  "(`Id` integer not null primary key autoincrement, `Name` varchar(256) not null, `Identifier` UNIQUEIDENTIFIER not null);");

            Db.Connection.Execute("CREATE TABLE `Reports`" +
                                  "(`Id` integer not null primary key autoincrement, `AnotherId` integer not null, `UserId` integer not null, " +
                                  "UNIQUE (`Id`, `AnotherId`));");


            InitData.Execute(Db);
        }
    }
}
