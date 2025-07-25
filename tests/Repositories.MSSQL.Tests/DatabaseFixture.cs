using System;
using Dapper;
using Repositories.Base;

namespace Repositories.MSSQL.Tests;

public abstract class DatabaseFixture : IDisposable
{
    protected DatabaseFixture(TestDbContext db)
    {
        Db = db;
        DropDatabase();
        InitDb();
    }

    private readonly string _dbName = "test_" + RandomGenerator.String();
    public TestDbContext Db { get; }

    private void InitDb()
    {
        Db.Connection.Execute($"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '{_dbName}') CREATE DATABASE [{_dbName}];");
        Db.Connection.Execute($"USE [{_dbName}]");

        void CreateSchema(string dbSchema)
        {
            Db.Connection.Execute($@"IF schema_id('{dbSchema}') IS NULL EXECUTE('CREATE SCHEMA {dbSchema}') ");
        }

        CreateSchema("DAB");

        Db.Connection.Execute(
            @"CREATE TABLE Users (Id int IDENTITY(1,1) not null, Name varchar(256) not null, AddressId int not null, PhoneId int not null, OfficePhoneId int not null, Deleted bit, UpdatedAt datetime2, PRIMARY KEY (Id))");
        Db.Connection.Execute(
            @"CREATE TABLE Cars (Id int IDENTITY(1,1) not null, Name varchar(256) not null, UserId int not null, Status int not null, Data binary(16) null, PRIMARY KEY (Id))");

        Db.Connection.Execute(@"CREATE TABLE Addresses (Id int IDENTITY(1,1) not null, Street varchar(256) not null, CityId varchar(256) not null,  PRIMARY KEY (Id))");
        Db.Connection.Execute(@"CREATE TABLE Cities (Identifier uniqueidentifier not null, Name varchar(256) not null)");
        Db.Connection.Execute(@"CREATE TABLE Reports (Id int not null, AnotherId int not null, UserId int not null,  PRIMARY KEY (Id, AnotherId))");
        Db.Connection.Execute(
            @"CREATE TABLE DAB.Phones (Id int IDENTITY(1,1) not null, PNumber varchar(256) not null, IsActive bit not null, Code varchar(256) not null, Deleted bit, PRIMARY KEY (Id))");

        InitData.Execute(Db);
    }

    private void DropDatabase()
    {
        Db.Connection.Execute($"USE master; DROP DATABASE IF EXISTS {_dbName}");
    }

    public void Dispose()
    {
        DropDatabase();
        Db.Dispose();
    }
}
