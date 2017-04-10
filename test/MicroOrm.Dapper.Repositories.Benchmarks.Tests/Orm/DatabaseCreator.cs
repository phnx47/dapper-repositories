using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests.Orm
{
    public static class DatabaseCreator
    {
        public static void CreateDb()
        {
            using (IDbConnection connection = new SqlConnection(Consts.ConnectionString.Replace(Consts.DbName, "master")))
            {
                connection.Open();
                connection.Execute($"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '{Consts.DbName}') CREATE DATABASE [{Consts.DbName}];");
                connection.Execute($"USE [{Consts.DbName}]");

                Action<string> dropTable = name => connection.Execute($@"IF OBJECT_ID('{name}', 'U') IS NOT NULL DROP TABLE [{name}]; ");
                dropTable("Users");

                connection.Execute(@"CREATE TABLE Users (Id int IDENTITY(1,1) not null, Name varchar(256) not null, PRIMARY KEY (Id))");

                for (var i = 0; i < 1000; i++)
                {
                    var name = "name" + i;
                    connection.Execute("INSERT INTO Users (Name) VALUES (@name)", new { name });
                }

                connection.Close();
            }
        }
    }
}