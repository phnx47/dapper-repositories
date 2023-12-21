using System;
using Dapper;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Npgsql;
using Repositories.Base;

namespace Repositories.PostgreSQL.Tests;

public class DatabaseFixture : IDisposable
{
    private readonly string _dbName = "test_" + RandomGenerator.String();
    private const string _masterConnString = "Host=localhost;Username=postgres;Password=Password12!";

    public DatabaseFixture()
    {
        Db = new TestDbContext(new NpgsqlConnection(_masterConnString + ";Database=" + _dbName), SqlProvider.PostgreSQL);
        DropDatabase();
        InitDb();
    }

    public TestDbContext Db { get; }

    public void Dispose()
    {
        Db.Dispose();
        DropDatabase();
    }

    protected void InitDb()
    {
        using var conn = new NpgsqlConnection(_masterConnString);
        conn.Execute($"CREATE DATABASE {_dbName};");

        void CreateSchema(string dbSchema)
        {
            Db.Connection.Execute($"CREATE SCHEMA {dbSchema} ");
        }

        CreateSchema("DAB");

        Db.Connection.Execute(
            @"CREATE TABLE Users (Id SERIAL not null, Name varchar(256) not null, AddressId int not null, PhoneId int not null, OfficePhoneId int not null, Deleted bool null, UpdatedAt timestamp, PRIMARY KEY (Id))");
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
        using var conn = new NpgsqlConnection(_masterConnString);
        conn.Execute($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE datname='{_dbName}' AND pid <> pg_backend_pid();");
        conn.Execute($"DROP DATABASE IF EXISTS {_dbName}");
    }
}
