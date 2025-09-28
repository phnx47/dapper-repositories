using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using TestClasses;
using Xunit;

namespace SqlGenerator.Tests;

public class MySQLGeneratorTests
{
    private const SqlProvider _sqlConnector = SqlProvider.MySQL;

    [Fact]
    public static void Count()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetCount(null);
        Assert.Equal("SELECT COUNT(*) FROM `Users` WHERE `Users`.`Deleted` IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinct()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetCount(null, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT `Users`.`AddressId`) FROM `Users` WHERE `Users`.`Deleted` IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinctAndWhere()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetCount(x => x.PhoneId == 1, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT `Users`.`AddressId`) FROM `Users` WHERE (`Users`.`PhoneId` = @PhoneId_p0) AND `Users`.`Deleted` IS NULL",
            sqlQuery.GetSql());
    }

    [Fact]
    public void BulkUpdate()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector);
        var phones = new List<Phone>
        {
            new() { Id = 10, IsActive = true, PNumber = "111" },
            new() { Id = 10, IsActive = false, PNumber = "222" }
        };

        var sqlQuery = sqlGenerator.GetBulkUpdate(phones);

        Assert.Equal("UPDATE DAB.Phones SET PNumber = @PNumber0, IsActive = @IsActive0, Deleted = @Deleted0 WHERE Id = @Id0; " +
                     "UPDATE DAB.Phones SET PNumber = @PNumber1, IsActive = @IsActive1, Deleted = @Deleted1 WHERE Id = @Id1", sqlQuery.GetSql());
    }

    [Fact]
    public void BulkUpdate_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, true);
        var phones = new List<Phone>
        {
            new() { Id = 10, IsActive = true, PNumber = "111" },
            new() { Id = 10, IsActive = false, PNumber = "222" }
        };

        var sqlQuery = sqlGenerator.GetBulkUpdate(phones);

        Assert.Equal("UPDATE `DAB`.`Phones` SET `PNumber` = @PNumber0, `IsActive` = @IsActive0, `Deleted` = @Deleted0 WHERE `Id` = @Id0; " +
                     "UPDATE `DAB`.`Phones` SET `PNumber` = @PNumber1, `IsActive` = @IsActive1, `Deleted` = @Deleted1 WHERE `Id` = @Id1", sqlQuery.GetSql());
    }

    [Fact]
    public void Update_Dictionary()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, true);
        var fieldDict = new Dictionary<string, object> { { "PNumber", "18573175437" } };

        var sqlQuery = sqlGenerator.GetUpdate(p => p.Id == 1, fieldDict);
        string sql = sqlQuery.GetSql();
        Assert.Equal("UPDATE `DAB`.`Phones` SET `DAB`.`Phones`.`PNumber` = @PhonePNumber WHERE `DAB`.`Phones`.`Id` = @Id_p0", sql);
    }

    [Fact]
    public void Update_Anonymous()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, true);

        var sqlQuery = sqlGenerator.GetUpdate(p => p.Id == 1, new { PNumber = "18573175437" });
        string sql = sqlQuery.GetSql();
        Assert.Equal("UPDATE `DAB`.`Phones` SET `DAB`.`Phones`.`PNumber` = @PhonePNumber WHERE `DAB`.`Phones`.`Id` = @Id_p0", sql);
    }

    [Fact]
    public void Insert_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetInsert(new Address());

        Assert.Equal("INSERT INTO `Addresses` (`Street`, `CityId`) VALUES (@Street, @CityId); SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS `Id`",
            sqlQuery.GetSql());
    }

    [Fact]
    public void Insert_AllowKeyAsIdentity_QuoMarks()
    {
        MicroOrmConfig.AllowKeyAsIdentity = true;
        var sqlGenerator = new SqlGenerator<AddressKeyAsIdentity>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetInsert(new AddressKeyAsIdentity());

        Assert.Equal("INSERT INTO `Addresses` (`Street`, `CityId`) VALUES (@Street, @CityId); SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS `Id`",
            sqlQuery.GetSql());
        MicroOrmConfig.AllowKeyAsIdentity = false;
    }

    [Fact]
    public void IsNotNull()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetSelectAll(user => user.UpdatedAt != null, null);

        Assert.Equal(
            "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
            "WHERE (`Users`.`UpdatedAt` IS NOT NULL) AND `Users`.`Deleted` IS NULL", sqlQuery.GetSql());
        Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());
    }

    [Fact]
    public void IsNotNullAnd()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetSelectAll(user => user.Name == "Frank" && user.UpdatedAt != null, null);

        Assert.Equal(
            "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
            "WHERE (`Users`.`Name` = @Name_p0 AND `Users`.`UpdatedAt` IS NOT NULL) AND `Users`.`Deleted` IS NULL", sqlQuery.GetSql());
        Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());

        sqlQuery = sqlGenerator.GetSelectAll(user => user.UpdatedAt != null && user.Name == "Frank", null);
        Assert.Equal(
            "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
            "WHERE (`Users`.`UpdatedAt` IS NOT NULL AND `Users`.`Name` = @Name_p1) AND `Users`.`Deleted` IS NULL", sqlQuery.GetSql());
        Assert.DoesNotContain("!= NULL", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectBetween()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetSelectBetween(1, 10, null, x => x.Id);

        Assert.Equal(
            "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` " +
            "WHERE `Users`.`Deleted` IS NULL AND `Users`.`Id` BETWEEN '1' AND '10'", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectById()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectById(1, null);
        Assert.Equal("SELECT Addresses.Id, Addresses.Street, Addresses.CityId FROM Addresses WHERE Addresses.Id = @Id LIMIT 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectByIdJoin()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetSelectById(1, null, q => q.Users);
        Assert.Equal(
            "SELECT `Addresses`.`Id`, `Addresses`.`Street`, `Addresses`.`CityId`, `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, " +
            "`Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Addresses` " +
            "LEFT JOIN `Users` ON `Addresses`.`Id` = `Users`.`AddressId` WHERE `Addresses`.`Id` = @Id", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectFirst()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty, null);
        Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 LIMIT 1", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectLimit()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
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
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
        var filterData = new FilterData();
        var data = filterData.OrderInfo ?? new OrderInfo();
        data.Columns = ["Name"];
        data.Direction = OrderInfo.SortDirection.ASC;
        filterData.OrderInfo = data;

        var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
        Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 ORDER BY Name ASC", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectOrderByWithTableIdentifier_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, true);
        var filterData = new FilterData();
        var data = filterData.OrderInfo ?? new OrderInfo();
        data.Columns = ["Cities.Name"];
        data.Direction = OrderInfo.SortDirection.ASC;
        filterData.OrderInfo = data;

        var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
        Assert.Equal("SELECT `Cities`.`Identifier`, `Cities`.`Name` FROM `Cities` WHERE `Cities`.`Identifier` = @Identifier_p0 ORDER BY `Cities`.`Name` ASC",
            sqlQuery.GetSql());
    }

    [Fact]
    public void SelectPaged()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
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
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty && x.Name == "", null);
        Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 AND Cities.Name = @Name_p1 LIMIT 1",
            sqlQuery.GetSql());
    }

    [Fact]
    public void SelectFirst_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty, null);
        Assert.Equal("SELECT `Cities`.`Identifier`, `Cities`.`Name` FROM `Cities` WHERE `Cities`.`Identifier` = @Identifier_p0 LIMIT 1", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectFirstWithDeleted()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Id == 6, null);
        Assert.Equal(
            "SELECT `Users`.`Id`, `Users`.`Name`, `Users`.`AddressId`, `Users`.`PhoneId`, `Users`.`OfficePhoneId`, `Users`.`Deleted`, `Users`.`UpdatedAt` FROM `Users` WHERE (`Users`.`Id` = @Id_p0) AND `Users`.`Deleted` IS NULL LIMIT 1",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateWithPredicateAndJoin()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var model = new User { Addresses = new Address() };
        var sqlQuery = sqlGenerator.GetUpdate(q => q.AddressId == 1, model, x => x.Addresses);
        var sql = sqlQuery.GetSql();
        Assert.Equal(
            "UPDATE Users LEFT JOIN Addresses ON Users.AddressId = Addresses.Id SET Users.Name = @UserName, Users.AddressId = @UserAddressId, Users.PhoneId = @UserPhoneId, Users.OfficePhoneId = @UserOfficePhoneId, Users.Deleted = @UserDeleted, Users.UpdatedAt = @UserUpdatedAt, Addresses.Street = @AddressStreet, Addresses.CityId = @AddressCityId WHERE Users.AddressId = @AddressId_p0",
            sql);
    }

    [Fact]
    public static void UpdateWithJoin()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var model = new User { Addresses = new Address() };
        var sqlQuery = sqlGenerator.GetUpdate(model, x => x.Addresses);
        var sql = sqlQuery.GetSql();
        Assert.Equal(
            "UPDATE Users LEFT JOIN Addresses ON Users.AddressId = Addresses.Id SET Users.Name = @UserName, Users.AddressId = @UserAddressId, Users.PhoneId = @UserPhoneId, Users.OfficePhoneId = @UserOfficePhoneId, Users.Deleted = @UserDeleted, Users.UpdatedAt = @UserUpdatedAt, Addresses.Street = @AddressStreet, Addresses.CityId = @AddressCityId WHERE Users.Id = @UserId",
            sql);
    }

    [Fact]
    public static void SetOrderByAndSetLimitWithWhere()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector);

        var filterData = new FilterData
        {
            OrderInfo = new OrderInfo { CustomQuery = "Identifier ASC, Name DESC " },
            LimitInfo = new LimitInfo { Limit = 10, Offset = 2 }
        };

        var sqlQuery = sqlGenerator.GetSelectAll(q => q.Name != "City", filterData);
        var sql = sqlQuery.GetSql();
        Assert.Equal(
            "SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Name != @Name_p0 ORDER BY Identifier ASC, Name DESC LIMIT 10 OFFSET 2",
            sql);
    }
}
