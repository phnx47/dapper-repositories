using System;
using System.Collections.Generic;
using System.Linq;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.Tests
{
    public class SqlGeneratorTests
    {
        [Fact]
        public void ChangeDate_Insert()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);

            var user = new User {Name = "Dude"};
            userSqlGenerator.GetInsert(user);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public void ChangeDate_Update()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);

            var user = new User {Name = "Dude"};
            userSqlGenerator.GetUpdate(user);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public void ExpressionArgumentException()
        {
            var list = new List<int>
            {
                6, 12
            };

            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);

            var isExceptions = false;

            try
            {
                userSqlGenerator.GetSelectAll(x => list.Contains(x.Id));
            }
            catch (ArgumentException ex)
            {
                Assert.Contains("Only one degree of nesting is supported", ex.Message);
                isExceptions = true;
            }

            Assert.True(isExceptions, "Contains no cast exception");
        }

        [Fact]
        public void MSSQLNavigationPredicate()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Phone.Number == "123", user => user.Phone);

            Assert.Equal("SELECT TOP 1 [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt], " +
                         "[DAB].[Phones].[Id], [DAB].[Phones].[Number], [DAB].[Phones].[IsActive] " +
                         "FROM [Users] INNER JOIN [DAB].[Phones] ON [Users].[PhoneId] = [DAB].[Phones].[Id] " +
                         "WHERE [DAB].[Phones].[Number] = @PhoneNumber AND [Users].[Deleted] != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MSSQLIsNull()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt == null);

            Assert.Equal("SELECT [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] FROM [Users] " +
                         "WHERE [Users].[UpdatedAt] IS NULL AND [Users].[Deleted] != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("== NULL", sqlQuery.GetSql());
        }

        [Fact]
        public void MSSQLJoinBracelets()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(null, user => user.Cars);

            Assert.Equal("SELECT [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt], " +
                         "[Cars].[Id], [Cars].[Name], [Cars].[Data], [Cars].[UserId], [Cars].[Status] " +
                         "FROM [Users] LEFT JOIN [Cars] ON [Users].[Id] = [Cars].[UserId] " +
                         "WHERE [Users].[Deleted] != 1", sqlQuery.GetSql());
        }


        [Fact]
        public void MSSQLSelectBetweenWithLogicalDelete()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, false);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.Deleted, Users.UpdatedAt FROM Users " +
                         "WHERE Users.Deleted != 1 AND Users.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MSSQLSelectBetweenWithLogicalDeleteBraclets()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] FROM [Users] " +
                         "WHERE [Users].[Deleted] != 1 AND [Users].[Id] BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }


        [Fact]
        public void MSSQLSelectBetweenWithoutLogicalDelete()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MSSQL, false);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT Addresses.Id, Addresses.Street, Addresses.CityId FROM Addresses " +
                         "WHERE Addresses.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MSSQLSelectBetweenWithoutLogicalDeleteBraclets()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT [Addresses].[Id], [Addresses].[Street], [Addresses].[CityId] FROM [Addresses] " +
                         "WHERE [Addresses].[Id] BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MSSQLSelectFirst()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 2);
            Assert.Equal("SELECT TOP 1 [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] FROM [Users] WHERE [Users].[Id] = @Id AND [Users].[Deleted] != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MySQLIsNotNull()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt != null);

            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                         "WHERE `Users`.`UpdatedAt` IS NOT NULL AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());
        }

        [Fact]
        public void MySQLIsNotNullAND()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(user => user.Name == "Frank" && user.UpdatedAt != null);

            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                         "WHERE `Users`.`Name` = @Name AND `Users`.`UpdatedAt` IS NOT NULL AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());

            sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt != null && user.Name == "Frank");
            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                         "WHERE `Users`.`UpdatedAt` IS NOT NULL AND `Users`.`Name` = @Name AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());
        }


        [Fact]
        public void MySQLSelectBetween()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                         "WHERE `Users`.`Deleted` != 1 AND `Users`.`Id` BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MySQLSelectFirst()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 6);
            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` WHERE `Users`.`Id` = @Id AND `Users`.`Deleted` != 1 LIMIT 1", sqlQuery.GetSql());
        }
    }
}