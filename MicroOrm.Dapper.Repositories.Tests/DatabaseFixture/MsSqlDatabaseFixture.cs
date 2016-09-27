using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MsSqlDatabaseFixture : IDisposable
    {
        private const string DbName = "test_micro_orm";

        public MsSqlDatabaseFixture()
        {
            var connString = "Server=(local);Initial Catalog=master;Integrated Security=True";

            if (Environments.IsAppVeyor)
            {
                connString = "Server=(local)\\SQL2016;Database=master;User ID=sa;Password=Password12!";
            }

            Db = new MSSqlDbContext(connString);

            InitDb();
        }

        public void Dispose()
        {
            Db.Connection.Execute($"USE master; DROP DATABASE {DbName}");
            Db.Dispose();
        }

        public MSSqlDbContext Db { get; }

        private void InitDb()
        {
            Db.Connection.Execute($"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '{DbName}') CREATE DATABASE [{DbName}];");
            Db.Connection.Execute($"USE [{DbName}]");
            Action<string> dropTable = name => Db.Connection.Execute($@"IF OBJECT_ID('{name}', 'U') IS NOT NULL DROP TABLE [{name}]; ");
            dropTable("Users");
            dropTable("Cars");

            Db.Connection.Execute(@"CREATE TABLE Users (Id int IDENTITY(1,1) not null, Name varchar(256) not null, Deleted bit not null, UpdatedAt datetime2, PRIMARY KEY (Id))");

            for (var i = 0; i < 10; i++)
            {
                Db.Users.Insert(new Classes.User()
                {
                    Name = $"TestName{i}"
                });
            }
            Db.Users.Insert(new Classes.User() { Name = $"TestName0" });

            Db.Connection.Execute(@"CREATE TABLE Cars (Id int IDENTITY(1,1) not null, Name varchar(256) not null, UserId int not null, Status int not null, PRIMARY KEY (Id))");

            Db.Cars.Insert(new Classes.Car() { Name = $"TestCar0", UserId = 1 });
        }
    }
}
