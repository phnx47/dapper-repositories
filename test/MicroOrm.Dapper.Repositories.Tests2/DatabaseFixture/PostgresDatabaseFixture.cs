using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;
using Npgsql;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class PostgresDatabaseFixture : IDisposable
    {

        private const string _dbName = "test_micro_orm";

        public PostgresDatabaseFixture()
        {
            Db = new PostgresDbContext("Host=localhost;Username=postgres;Password=Password12!");

            DropDatabase();

            InitDb();
        }

        public PostgresDbContext Db { get; }

        public void Dispose()
        {
            Db.SetConnectionString("Host=localhost;Username=postgres;Password=Password12!");

            DropDatabase();
            Db.Dispose();
        }

        protected void InitDb()
        {
            Db.Connection.Execute($"CREATE DATABASE {_dbName};");

            Db.SetConnectionString("Host=localhost;Username=postgres;Password=Password12!;Database=" + _dbName);

            void CreateSchema(string dbSchema)
            {
                Db.Connection.Execute($"CREATE SCHEMA {dbSchema} ");
            }

            CreateSchema("DAB");

            Db.Connection.Execute(
                @"CREATE TABLE Users (Id SERIAL not null, Name varchar(256) not null, AddressId int not null, PhoneId int not null, OfficePhoneId int not null, Deleted bool not null, UpdatedAt timestamp, PRIMARY KEY (Id))");
            Db.Connection.Execute(
                @"CREATE TABLE Cars (Id SERIAL not null, Name varchar(256) not null, UserId int not null, Status int not null, Data bytea null, PRIMARY KEY (Id))");

            Db.Connection.Execute(@"CREATE TABLE Addresses (Id SERIAL not null, Street varchar(256) not null, CityId varchar(256) not null,  PRIMARY KEY (Id))");
            Db.Connection.Execute(@"CREATE TABLE Cities (Identifier uuid not null, Name varchar(256) not null)");
            Db.Connection.Execute(@"CREATE TABLE Reports (Id int not null, AnotherId int not null, UserId int not null,  PRIMARY KEY (Id, AnotherId))");
            Db.Connection.Execute(
                @"CREATE TABLE DAB.Phones (Id SERIAL not null, PNumber varchar(256) not null, IsActive bool not null, Code varchar(256) not null, PRIMARY KEY (Id))");

            InitData.Execute(Db);
        }

        private void DropDatabase()
        {
            Db.Connection.Execute($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE datname='{_dbName}' AND pid <> pg_backend_pid();");
            Db.Connection.Execute($"DROP DATABASE IF EXISTS {_dbName}");
        }
    }
}
