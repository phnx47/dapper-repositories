using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MsSqlDatabaseFixture : IDisposable
    {
        private const string _dbName = "test_micro_orm";

        public MsSqlDatabaseFixture()
        {
            var connString = "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;";

            if (Environments.IsAppVeyor)
                connString = "Server=(local)\\SQL2017;Database=master;User ID=sa;Password=Password12!";

            Db = new MsSqlDbContext(connString);

            InitDb();
        }

        public MsSqlDbContext Db { get; }

        public void Dispose()
        {
            Db.Connection.Execute($"USE master; DROP DATABASE {_dbName}");
            Db.Dispose();
        }

        private void InitDb()
        {
            Db.Connection.Execute($"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '{_dbName}') CREATE DATABASE [{_dbName}];");
            Db.Connection.Execute($"USE [{_dbName}]");

            ClearDb();

            void CreateSchema(string dbSchema)
            {
                Db.Connection.Execute($@"IF schema_id('{dbSchema}') IS NULL EXECUTE('CREATE SCHEMA {dbSchema}') ");
            }

            CreateSchema("DAB");

            Db.Connection.Execute(@"CREATE TABLE Users (Id int IDENTITY(1,1) not null, Name varchar(256) not null, AddressId int not null, PhoneId int not null, OfficePhoneId int not null, Deleted bit not null, UpdatedAt datetime2, PRIMARY KEY (Id))");
            Db.Connection.Execute(@"CREATE TABLE Cars (Id int IDENTITY(1,1) not null, Name varchar(256) not null, UserId int not null, Status int not null, Data binary(16) null, PRIMARY KEY (Id))");

            Db.Connection.Execute(@"CREATE TABLE Addresses (Id int IDENTITY(1,1) not null, Street varchar(256) not null, CityId varchar(256) not null,  PRIMARY KEY (Id))");
            Db.Connection.Execute(@"CREATE TABLE Cities (Identifier uniqueidentifier not null, Name varchar(256) not null)");
            Db.Connection.Execute(@"CREATE TABLE Reports (Id int not null, AnotherId int not null, UserId int not null,  PRIMARY KEY (Id, AnotherId))");
            Db.Connection.Execute(@"CREATE TABLE DAB.Phones (Id int IDENTITY(1,1) not null, Number varchar(256) not null, IsActive bit not null, Code varchar(256) not null, PRIMARY KEY (Id))");

            InitData.Execute(Db);
        }

        private void ClearDb()
        {    
            void DropTable(string schema, string name)
            {
                Db.Connection.Execute($@"IF OBJECT_ID('{schema}.{name}', 'U') IS NOT NULL DROP TABLE [{schema}].[{name}]; ");
            }

            DropTable("dbo", "Users");
            DropTable("dbo", "Cars");
            DropTable("dbo", "Addresses");
            DropTable("dbo", "Cities");
            DropTable("dbo", "Reports");
            DropTable("DAB", "Phones");
        }
    }
}
