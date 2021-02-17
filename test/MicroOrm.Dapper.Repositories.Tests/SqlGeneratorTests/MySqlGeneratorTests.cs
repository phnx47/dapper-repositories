using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.SqlGeneratorTests
{
    public class MySqlGeneratorTests
    {
        private const SqlProvider _sqlConnector = SqlProvider.MySQL;

        [Fact]
        public static void Count()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetCount(null);
            Assert.Equal("SELECT COUNT(*) FROM `Users` WHERE `Users`.`Deleted` != 1", sqlQuery.GetSql());
        }

        [Fact]
        public static void CountWithDistinct()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetCount(null, user => user.AddressId);
            Assert.Equal("SELECT COUNT(DISTINCT `Users`.`AddressId`) FROM `Users` WHERE `Users`.`Deleted` != 1", sqlQuery.GetSql());
        }

        [Fact]
        public static void CountWithDistinctAndWhere()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetCount(x => x.PhoneId == 1, user => user.AddressId);
            Assert.Equal("SELECT COUNT(DISTINCT `Users`.`AddressId`) FROM `Users` WHERE (`Users`.`PhoneId` = @PhoneId_p0) AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void BulkUpdate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector);
            var phones = new List<Phone>
            {
                new Phone {Id = 10, IsActive = true, Number = "111"},
                new Phone {Id = 10, IsActive = false, Number = "222"}
            };

            var sqlQuery = userSqlGenerator.GetBulkUpdate(phones);

            Assert.Equal("UPDATE DAB.Phones SET Number = @Number0, IsActive = @IsActive0 WHERE Id = @Id0; " +
                         "UPDATE DAB.Phones SET Number = @Number1, IsActive = @IsActive1 WHERE Id = @Id1", sqlQuery.GetSql());
        }

        [Fact]
        public void BulkUpdate_QuoMarks()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, true);
            var phones = new List<Phone>
            {
                new Phone {Id = 10, IsActive = true, Number = "111"},
                new Phone {Id = 10, IsActive = false, Number = "222"}
            };

            var sqlQuery = userSqlGenerator.GetBulkUpdate(phones);

            Assert.Equal("UPDATE `DAB`.`Phones` SET `Number` = @Number0, `IsActive` = @IsActive0 WHERE `Id` = @Id0; " +
                         "UPDATE `DAB`.`Phones` SET `Number` = @Number1, `IsActive` = @IsActive1 WHERE `Id` = @Id1", sqlQuery.GetSql());
        }

        [Fact]
        public void Update_Dictionary()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, true);
            Dictionary<string, object> fieldDict = new Dictionary<string, object>();
            fieldDict.Add("Number", "18573175437");

            var sqlQuery = userSqlGenerator.GetUpdate(p => p.Id == 1, fieldDict);
            string sql = sqlQuery.GetSql();
            Assert.Equal("UPDATE `DAB`.`Phones` SET `DAB`.`Phones`.`Number` = @PhoneNumber WHERE `DAB`.`Phones`.`Id` = @Id_p0", sql);
        }

        [Fact]
        public void Update_Anonymous()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, true);

            var sqlQuery = userSqlGenerator.GetUpdate(p => p.Id == 1, new
            {
                Number = "18573175437"
            });
            string sql = sqlQuery.GetSql();
            Assert.Equal("UPDATE `DAB`.`Phones` SET `DAB`.`Phones`.`Number` = @PhoneNumber WHERE `DAB`.`Phones`.`Id` = @Id_p0", sql);
        }

        [Fact]
        public void Insert_QuoMarks()
        {
            ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetInsert(new Address());

            Assert.Equal("INSERT INTO `Addresses` (`Street`, `CityId`) VALUES (@Street, @CityId); SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS `Id`", sqlQuery.GetSql());
        }

        [Fact]
        public void IsNotNull()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt != null, null);
            var sqlQuery2 = userSqlGenerator.GetSelectAll(user => !user.UpdatedAt.HasValue, null);

            Assert.Equal(
                "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                "WHERE (`Users`.`UpdatedAt` IS NOT NULL) AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());
        }

        [Fact]
        public void IsNotNullAnd()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetSelectAll(user => user.Name == "Frank" && user.UpdatedAt != null, null);

            Assert.Equal(
                "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                "WHERE (`Users`.`Name` = @Name_p0 AND `Users`.`UpdatedAt` IS NOT NULL) AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());

            sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt != null && user.Name == "Frank", null);
            Assert.Equal(
                "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                "WHERE (`Users`.`UpdatedAt` IS NOT NULL AND `Users`.`Name` = @Name_p1) AND `Users`.`Deleted` != 1", sqlQuery.GetSql());
            Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectBetween()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, null, x => x.Id);

            Assert.Equal(
                "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
                "WHERE `Users`.`Deleted` != 1 AND `Users`.`Id` BETWEEN '1' AND '10'", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectById()
        {
            ISqlGenerator<Address> sqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
            var sqlQuery = sqlGenerator.GetSelectById(1, null);
            Assert.Equal("SELECT Addresses.Id, Addresses.Street, Addresses.CityId FROM Addresses WHERE Addresses.Id = @Id LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectFirst()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty, null);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectLimit()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var filterData = new FilterData();
            var data = filterData.LimitInfo ?? new LimitInfo();
            data.Limit = 10u;
            filterData.LimitInfo = data;

            var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 LIMIT 10", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectOrderBy()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var filterData = new FilterData();
            var data = filterData.OrderInfo ?? new OrderInfo();
            data.Columns = new List<string> { "Name" };
            data.Direction = OrderInfo.SortDirection.ASC;
            filterData.OrderInfo = data;

            var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 ORDER BY Name ASC", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectPaged()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var filterData = new FilterData();
            var data = filterData.LimitInfo ?? new LimitInfo();
            data.Limit = 10u;
            data.Offset = 5u;
            filterData.LimitInfo = data;

            var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 LIMIT 10 OFFSET 5", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectFirst2()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty && x.Name == "", null);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 AND Cities.Name = @Name_p1 LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectFirst_QuoMarks()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, true);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty, null);
            Assert.Equal("SELECT `Cities`.`Identifier`, `Cities`.`Name` FROM `Cities` WHERE `Cities`.`Identifier` = @Identifier_p0 LIMIT 1", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectFirstWithDeleted()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 6, null);
            Assert.Equal(
                "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` WHERE (`Users`.`Id` = @Id_p0) AND `Users`.`Deleted` != 1 LIMIT 1",
                sqlQuery.GetSql());
        }

        [Fact]
        public static void UpdateWithPredicateAndJoin()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(_sqlConnector);
            var model = new User { Addresses = new Address() };
            var sqlQuery = sqlGenerator.GetUpdate(q => q.AddressId == 1, model, x => x.Addresses);
            var sql = sqlQuery.GetSql();
            Assert.Equal("UPDATE Users LEFT JOIN Addresses ON Users.AddressId = Addresses.Id SET Users.Name = @UserName, Users.AddressId = @UserAddressId, Users.PhoneId = @UserPhoneId, Users.OfficePhoneId = @UserOfficePhoneId, Users.Deleted = @UserDeleted, Users.UpdatedAt = @UserUpdatedAt, Addresses.Street = @AddressStreet, Addresses.CityId = @AddressCityId WHERE Users.AddressId = @AddressId_p0", sql);
        }

        [Fact]
        public static void UpdateWithJoin()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(_sqlConnector);
            var model = new User { Addresses = new Address() };
            var sqlQuery = sqlGenerator.GetUpdate(model, x => x.Addresses);
            var sql = sqlQuery.GetSql();
            Assert.Equal("UPDATE Users LEFT JOIN Addresses ON Users.AddressId = Addresses.Id SET Users.Name = @UserName, Users.AddressId = @UserAddressId, Users.PhoneId = @UserPhoneId, Users.OfficePhoneId = @UserOfficePhoneId, Users.Deleted = @UserDeleted, Users.UpdatedAt = @UserUpdatedAt, Addresses.Street = @AddressStreet, Addresses.CityId = @AddressCityId WHERE Users.Id = @UserId", sql);
        }
    }
}
