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

            var user = new User { Name = "Dude" };
            userSqlGenerator.GetInsert(user);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public void ChangeDate_Update()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);

            var user = new User { Name = "Dude" };
            userSqlGenerator.GetUpdate(user);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public void ExpressionArgumentException()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);

            var isExceptions = false;

            try
            {
                var sumAr = new List<int> { 1, 2, 3 };
                userSqlGenerator.GetSelectAll(x => sumAr.All(z => x.Id == z));
            }
            catch (NotImplementedException ex)
            {
                Assert.Contains("'All' method is not implemented", ex.Message);
                isExceptions = true;
            }

            Assert.True(isExceptions, "Contains no cast exception");
        }

        [Fact]
        public void MsSqlBoolFalseEqualNotPredicate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.IsActive != true);

            var parameters = sqlQuery.Param as IDictionary<string, object>;
            Assert.True(Convert.ToBoolean(parameters["IsActive"]));

            Assert.Equal("SELECT TOP 1 [DAB].[Phones].[Id], [DAB].[Phones].[Number], [DAB].[Phones].[IsActive], [DAB].[Phones].[Code] FROM [DAB].[Phones] WHERE [DAB].[Phones].[IsActive] != @IsActive", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlBoolFalseEqualPredicate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.IsActive == false);

            var parameters = sqlQuery.Param as IDictionary<string, object>;
            Assert.False(Convert.ToBoolean(parameters["IsActive"]));

            Assert.Equal("SELECT TOP 1 [DAB].[Phones].[Id], [DAB].[Phones].[Number], [DAB].[Phones].[IsActive], [DAB].[Phones].[Code] FROM [DAB].[Phones] WHERE [DAB].[Phones].[IsActive] = @IsActive", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlBoolFalsePredicate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => !x.IsActive);

            var parameters = sqlQuery.Param as IDictionary<string, object>;
            Assert.False(Convert.ToBoolean(parameters["IsActive"]));

            Assert.Equal("SELECT TOP 1 [DAB].[Phones].[Id], [DAB].[Phones].[Number], [DAB].[Phones].[IsActive], [DAB].[Phones].[Code] FROM [DAB].[Phones] WHERE [DAB].[Phones].[IsActive] = @IsActive", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlBoolTruePredicate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.IsActive);

            var parameters = sqlQuery.Param as IDictionary<string, object>;
            Assert.True(Convert.ToBoolean(parameters["IsActive"]));

            Assert.Equal("SELECT TOP 1 [DAB].[Phones].[Id], [DAB].[Phones].[Number], [DAB].[Phones].[IsActive], [DAB].[Phones].[Code] FROM [DAB].[Phones] WHERE [DAB].[Phones].[IsActive] = @IsActive", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlBulkInsertMultiple()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetBulkInsert(new List<Address> { new Address(), new Address() });

            Assert.Equal("INSERT INTO [Addresses] ([Street], [CityId]) VALUES (@Street0, @CityId0),(@Street1, @CityId1)", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlBulkInsertOne()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetBulkInsert(new List<Address> { new Address() });

            Assert.Equal("INSERT INTO [Addresses] ([Street], [CityId]) VALUES (@Street0, @CityId0)", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlContainsPredicate()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var ids = new List<int>();
            var sqlQuery = userSqlGenerator.GetSelectAll(x => ids.Contains(x.Id));

            Assert.Equal("SELECT [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] " +
                         "FROM [Users] WHERE [Users].[Id] IN @Id AND [Users].[Deleted] != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlInsertQuoMarks()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetInsert(new Address());

            Assert.Equal("INSERT INTO [Addresses] ([Street], [CityId]) VALUES (@Street, @CityId) SELECT SCOPE_IDENTITY() AS [Id]", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlIsNull()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt == null);

            Assert.Equal("SELECT [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] FROM [Users] " +
                         "WHERE [Users].[UpdatedAt] IS NULL AND [Users].[Deleted] != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("== NULL", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlJoinBracelets()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(null, user => user.Cars);

            Assert.Equal("SELECT [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt], " +
                         "[Cars].[Id], [Cars].[Name], [Cars].[Data], [Cars].[UserId], [Cars].[Status] " +
                         "FROM [Users] LEFT JOIN [Cars] ON [Users].[Id] = [Cars].[UserId] " +
                         "WHERE [Users].[Deleted] != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlNavigationPredicate()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Phone.Number == "123", user => user.Phone);

            Assert.Equal("SELECT TOP 1 [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt], " +
                         "[DAB].[Phones].[Id], [DAB].[Phones].[Number], [DAB].[Phones].[IsActive], [DAB].[Phones].[Code] " +
                         "FROM [Users] INNER JOIN [DAB].[Phones] ON [Users].[PhoneId] = [DAB].[Phones].[Id] " +
                         "WHERE [DAB].[Phones].[Number] = @PhoneNumber AND [Users].[Deleted] != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlNavigationPredicateNoQuotationMarks()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, false);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Phone.Number == "123", user => user.Phone);

            Assert.Equal("SELECT TOP 1 Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.Deleted, Users.UpdatedAt, " +
                         "DAB.Phones.Id, DAB.Phones.Number, DAB.Phones.IsActive, DAB.Phones.Code " +
                         "FROM Users INNER JOIN DAB.Phones ON Users.PhoneId = DAB.Phones.Id " +
                         "WHERE DAB.Phones.Number = @PhoneNumber AND Users.Deleted != 1", sqlQuery.GetSql());
        }


        [Fact]
        public void MsSqlSelectBetweenWithLogicalDelete()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, false);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.Deleted, Users.UpdatedAt FROM Users " +
                         "WHERE Users.Deleted != 1 AND Users.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlSelectBetweenWithLogicalDeleteBraclets()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] FROM [Users] " +
                         "WHERE [Users].[Deleted] != 1 AND [Users].[Id] BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }


        [Fact]
        public void MsSqlSelectBetweenWithoutLogicalDelete()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MSSQL, false);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT Addresses.Id, Addresses.Street, Addresses.CityId FROM Addresses " +
                         "WHERE Addresses.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlSelectBetweenWithoutLogicalDeleteBraclets()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT [Addresses].[Id], [Addresses].[Street], [Addresses].[CityId] FROM [Addresses] " +
                         "WHERE [Addresses].[Id] BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlSelectById()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectById(1);

            Assert.Equal("SELECT TOP 1 [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] " +
                         "FROM [Users] WHERE [Users].[Id] = @Id AND [Users].[Deleted] != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSQLSelectFirst()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 2);
            Assert.Equal("SELECT TOP 1 [Users].[Id], [Users].[Name], [Users].[AddressId], [Users].[PhoneId], [Users].[Deleted], [Users].[UpdatedAt] FROM [Users] WHERE [Users].[Id] = @Id AND [Users].[Deleted] != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlUpdateExclude()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var phone = new Phone { Id = 10, Code = "ZZZ", IsActive = true, Number = "111" };
            var sqlQuery = userSqlGenerator.GetUpdate(phone);

            Assert.Equal("UPDATE [DAB].[Phones] SET [Number] = @Number, [IsActive] = @IsActive WHERE [Id] = @Id", sqlQuery.GetSql());
        }

        [Fact]
        public void MySqlInsertQuoMarks()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetInsert(new Address());

            Assert.Equal("INSERT INTO `Addresses` (`Street`, `CityId`) VALUES (@Street, @CityId); SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS `Id`", sqlQuery.GetSql());
        }

        [Fact]
        public void MySqlIsNotNull()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt != null);

            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                         "WHERE `Users`.`UpdatedAt` IS NOT NULL AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());
        }

        [Fact]
        public void MySqlIsNotNullAND()
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
        public void MySqlSelectBetween()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, x => x.Id);

            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                         "WHERE `Users`.`Deleted` != 1 AND `Users`.`Id` BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void MySqlSelectFirstWithDeleted()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 6);
            Assert.Equal("SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` WHERE `Users`.`Id` = @Id AND `Users`.`Deleted` != 1 LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MySqlSelectFirst()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(ESqlConnector.MySQL, false);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == "");
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void PostgreSqlSelectFirst()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(ESqlConnector.PostgreSQL, false);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == "");
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MySqlSelectFirstQuoMarks()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(ESqlConnector.MySQL, true);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == "");
            Assert.Equal("SELECT `Cities`.`Identifier`, `Cities`.`Name` FROM `Cities` WHERE `Cities`.`Identifier` = @Identifier LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void PostgreSqlSelectFirstQuoMarks()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(ESqlConnector.PostgreSQL, true);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == "");
            Assert.Equal("SELECT \"Cities\".\"Identifier\", \"Cities\".\"Name\" FROM \"Cities\" WHERE \"Cities\".\"Identifier\" = @Identifier LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlDelete()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var phone = new Phone { Id = 10, Code = "ZZZ", IsActive = true, Number = "111" };
            var sqlQuery = userSqlGenerator.GetDelete(phone);

            Assert.Equal("DELETE FROM [DAB].[Phones] WHERE [Id] = @Id", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlDeleteWithSinglePredicate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetDeleteAll(x => x.IsActive);

            Assert.Equal("DELETE FROM [DAB].[Phones] WHERE [DAB].[Phones].[IsActive] = @IsActive", sqlQuery.GetSql());
        }


        [Fact]
        public void MsSqlDeleteWithMultiplePredicate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            var sqlQuery = userSqlGenerator.GetDeleteAll(x => x.IsActive && x.Number == "111");

            Assert.Equal("DELETE FROM [DAB].[Phones] WHERE [DAB].[Phones].[IsActive] = @IsActive AND [DAB].[Phones].[Number] = @Number", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlBulkUpdate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MSSQL, true);
            List<Phone> phones = new List<Phone>
            {
                new Phone { Id = 10, IsActive = true, Number = "111" },
                new Phone { Id = 10, IsActive = false, Number = "222" }
            };

            var sqlQuery = userSqlGenerator.GetBulkUpdate(phones);

            Assert.Equal(" UPDATE [DAB].[Phones] SET [Number] = @Number0, [IsActive] = @IsActive0 WHERE [Id] = @Id0" +
                         " UPDATE [DAB].[Phones] SET [Number] = @Number1, [IsActive] = @IsActive1 WHERE [Id] = @Id1", sqlQuery.GetSql());
        }

        [Fact]
        public void MySqlBulkUpdate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(ESqlConnector.MySQL, true);
            List<Phone> phones = new List<Phone>
            {
                new Phone { Id = 10, IsActive = true, Number = "111" },
                new Phone { Id = 10, IsActive = false, Number = "222" }
            };

            var sqlQuery = userSqlGenerator.GetBulkUpdate(phones);

            Assert.Equal(" UPDATE `DAB`.`Phones` SET `Number` = @Number0, `IsActive` = @IsActive0 WHERE `Id` = @Id0" +
                         " UPDATE `DAB`.`Phones` SET `Number` = @Number1, `IsActive` = @IsActive1 WHERE `Id` = @Id1", sqlQuery.GetSql());
        }

        [Fact]
        public void MsSqlBulkUpdateIgnoreOneOfKeys()
        {
            ISqlGenerator<Report> userSqlGenerator = new SqlGenerator<Report>(ESqlConnector.MSSQL, true);
            List<Report> reports = new List<Report>
            {
                new Report { Id = 10, AnotherId = 10, UserId = 22 },
                new Report { Id = 10, AnotherId = 10, UserId = 23 }
            };

            var sqlQuery = userSqlGenerator.GetBulkUpdate(reports);

            Assert.Equal(" UPDATE [Reports] SET [UserId] = @UserId0 WHERE [AnotherId] = @AnotherId0" +
                         " UPDATE [Reports] SET [UserId] = @UserId1 WHERE [AnotherId] = @AnotherId1", sqlQuery.GetSql());
        }
    }
}