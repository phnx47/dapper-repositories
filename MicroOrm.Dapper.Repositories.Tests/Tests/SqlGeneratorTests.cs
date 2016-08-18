using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.Tests
{
    public class SqlGeneratorTests
    {

        public SqlGeneratorTests()
        {

        }


        [Fact]
        public void MSSQLSelectFirst()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 2);
            Assert.Equal(sqlQuery.Sql, "SELECT TOP 1 [Users].[Id], [Users].[Name], [Users].[Deleted], [Users].[UpdatedAt] FROM [Users] WHERE [Users].[Id] = @Id AND [Users].[Deleted] != 1");
        }

        [Fact]
        public void MySQLSelectFirst()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 6);
            Assert.Equal(sqlQuery.Sql, "SELECT Users.Id, Users.Name, Users.Deleted, Users.UpdatedAt FROM Users WHERE Users.Id = @Id AND Users.Deleted != 1 LIMIT 1");
        }

        [Fact]
        public void ContainsExpression()
        {
            List<int> list = new List<int>()
            {
                6, 12
            };

            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);

            var isExceptions = false;

            try
            {
                userSqlGenerator.GetSelectAll(x => list.Contains(x.Id));
            }
            catch (NotImplementedException ex)
            {
                Assert.Contains(ex.Message, "predicate can't parse");
                isExceptions = true;

            }

            Assert.True(isExceptions, "Contains no cast exception");
        }

        [Fact]
        public void ChangeDate_Update()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);

            var user = new User() { Name = "Dude" };
            userSqlGenerator.GetUpdate(user);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public void ChangeDate_Insert()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);

            var user = new User() { Name = "Dude" };
            userSqlGenerator.GetInsert(user);
            Assert.NotNull(user.UpdatedAt);
        }

       
    }
}
