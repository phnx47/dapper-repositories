using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using TestClasses;
using Xunit;

namespace SqlGenerator.Tests;

public class SQLiteGeneratorTests
{
    private const SqlProvider _sqlConnector = SqlProvider.SQLite;

    [Fact]
    public void Count()
    {
        var userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = userSqlGenerator.GetCount(null);
        Assert.Equal("SELECT COUNT(*) FROM `Users` WHERE `Users`.`Deleted` IS NULL", sqlQuery.GetSql());
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
    public static void UpdateWithJoinThrows()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Id = 10, Name = "John", Addresses = new Address() };
        var ex = Assert.Throws<NotSupportedException>(() => sqlGenerator.GetUpdate(user, x => x.Addresses));

        Assert.Contains("only for MySQL", ex.Message);
    }

    [Fact]
    public static void UpdateColumns()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Id = 10, Name = "John", AddressId = 5 };
        var sqlQuery = sqlGenerator.GetUpdateColumns(user, x => x.Name);

        Assert.Equal("UPDATE Users SET Name = @UserName, UpdatedAt = @UserUpdatedAt WHERE Users.Id = @UserId", sqlQuery.GetSql());
        Assert.NotNull(user.UpdatedAt);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.Equal(3, parameters.Count);
        Assert.Equal("John", parameters["UserName"]);
        Assert.Equal(10, parameters["UserId"]);
    }

    [Fact]
    public static void UpdateColumns_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetUpdateColumns(new User { Id = 10, Name = "John" }, x => x.Name);

        Assert.Equal("UPDATE `Users` SET `Name` = @UserName, `UpdatedAt` = @UserUpdatedAt WHERE `Users`.`Id` = @UserId", sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateColumnsWithPredicate()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Name = "John", AddressId = 5 };
        var sqlQuery = sqlGenerator.GetUpdateColumns(x => x.Id == 10, user, x => x.Name, x => x.AddressId);

        Assert.Equal("UPDATE Users SET Name = @UserName, AddressId = @UserAddressId, UpdatedAt = @UserUpdatedAt WHERE Users.Id = @Id_p0",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateColumnsWithDuplicate()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetUpdateColumns(new User { Id = 10, Name = "John" }, x => x.Name, x => x.Name);

        Assert.Equal("UPDATE Users SET Name = @UserName, UpdatedAt = @UserUpdatedAt WHERE Users.Id = @UserId", sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateColumnsWithKeyThrows()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var ex = Assert.Throws<ArgumentException>(() => sqlGenerator.GetUpdateColumns(new User { Id = 10, Name = "John" }, x => x.Id));

        Assert.Contains("Can't update [Id]", ex.Message);
    }

    [Fact]
    public static void UpdateColumnsWithIgnoreUpdateThrows()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector);
        var ex = Assert.Throws<ArgumentException>(() => sqlGenerator.GetUpdateColumns(new Phone { Id = 10, Code = "ZZZ" }, x => x.Code));

        Assert.Contains("[IgnoreUpdate]", ex.Message);
    }

    [Fact]
    public static void UpdateColumnsWithReadOnlyColumnThrows()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var ex = Assert.Throws<ArgumentException>(() => sqlGenerator.GetUpdateColumns(new User { Id = 10, Name = "John" }, x => x.DisplayName));

        Assert.Contains("not a mapped column", ex.Message);
    }

    [Fact]
    public static void UpdateAnonymousWithUpdatedAt()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetUpdate(x => x.Id == 10, new { Name = "John" });

        Assert.Equal("UPDATE Users SET Name = @UserName, UpdatedAt = @UserUpdatedAt WHERE Users.Id = @Id_p0", sqlQuery.GetSql());

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.IsType<DateTime>(parameters["UserUpdatedAt"]);
    }

    [Fact]
    public static void UpdateDictionaryWithUpdatedAt()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetUpdate(x => x.Id == 10, new Dictionary<string, object> { { "Name", "John" } });

        Assert.Equal("UPDATE Users SET Name = @UserName, UpdatedAt = @UserUpdatedAt WHERE Users.Id = @Id_p0", sqlQuery.GetSql());

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.IsType<DateTime>(parameters["UserUpdatedAt"]);
    }
}
