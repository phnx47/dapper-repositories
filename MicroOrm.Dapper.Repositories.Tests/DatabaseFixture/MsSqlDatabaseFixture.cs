using System;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;

namespace MicroOrm.Dapper.Repositories.Tests.DatabaseFixture
{
    public class MsSqlDatabaseFixture : IDisposable
    {
        public MsSqlDatabaseFixture()
        {
            var connString = "Server=(local);Initial Catalog=microorm_test;Integrated Security=True";

            if (Environments.IsAppVeyor)
            {
                connString = @"Server=(local)\SQL2014;Database=tempdb;User ID=sa;Password=Password12!";
            }

            Db = new MSSqlDbContext(connString);

            InitDb();
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public MSSqlDbContext Db { get; }

        private void InitDb()
        {
            Action<string> dropTable = name => Db.Connection.Execute($@"IF OBJECT_ID('{name}', 'U') IS NOT NULL DROP TABLE [{name}]; ");
            dropTable("Users");
            dropTable("Cars");
           
            Db.Connection.Execute(@"CREATE TABLE Users (Id int IDENTITY(1,1) not null, Name varchar(256) not null, Deleted bit not null, PRIMARY KEY (Id))");
            for (var i = 0; i < 10; i++)
            {
                Db.Connection.Execute($"INSERT INTO [dbo].[Users]([Name],[Deleted])VALUES('TestName{i}', 0)");
            }
            Db.Connection.Execute("INSERT INTO [dbo].[Users]([Name],[Deleted])VALUES(\'TestName0\', 0)");

            Db.Connection.Execute(@"CREATE TABLE Cars (Id int IDENTITY(1,1) not null, Name varchar(256) not null, UserId int not null, Status int not null, PRIMARY KEY (Id))");
            Db.Connection.Execute("INSERT INTO [dbo].[Cars]([Name],[UserId],[Status])VALUES('TestCar0', 1, 0)");
        }
    }
}
