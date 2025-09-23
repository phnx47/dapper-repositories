using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using TestClasses;
using Xunit;

namespace SqlGenerator.Tests;

public class PostgreSQLGeneratorTests
{
    private const SqlProvider _sqlConnector = SqlProvider.PostgreSQL;

    [Fact]
    public static void Count()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetCount(null);
        Assert.Equal("SELECT COUNT(*) FROM \"Users\" WHERE \"Users\".\"Deleted\" IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinct()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetCount(null, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT \"Users\".\"AddressId\") FROM \"Users\" WHERE \"Users\".\"Deleted\" IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinctAndWhere()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetCount(x => x.PhoneId == 1, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT \"Users\".\"AddressId\") FROM \"Users\" WHERE (\"Users\".\"PhoneId\" = @PhoneId_p0) AND \"Users\".\"Deleted\" IS NULL",
            sqlQuery.GetSql());
    }

    [Fact]
    public void SelectLimit()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector);
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
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector);
        var filterData = new FilterData();
        var data = filterData.OrderInfo ?? new OrderInfo();
        data.Columns = new List<string> { "Name" };
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
        data.Columns = new List<string> { "Cities.Name" };
        data.Direction = OrderInfo.SortDirection.ASC;
        filterData.OrderInfo = data;

        var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
        Assert.Equal("SELECT \"Cities\".\"Identifier\", \"Cities\".\"Name\" FROM \"Cities\" WHERE \"Cities\".\"Identifier\" = @Identifier_p0 ORDER BY \"Cities\".\"Name\" ASC",
            sqlQuery.GetSql());
    }

    [Fact]
    public void SelectPaged()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector);
        var filterData = new FilterData();
        var data = filterData.LimitInfo ?? new LimitInfo();
        data.Limit = 10u;
        data.Offset = 5u;
        filterData.LimitInfo = data;

        var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
        Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 LIMIT 10 OFFSET 5", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectFirst()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty, null);
        Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 LIMIT 1", sqlQuery.GetSql());
    }

    [Fact]
    public void SelectFirst_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty, null);
        Assert.Equal("SELECT \"Cities\".\"Identifier\", \"Cities\".\"Name\" FROM \"Cities\" WHERE \"Cities\".\"Identifier\" = @Identifier_p0 LIMIT 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void Update()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetUpdate(new User());

        Assert.Equal("UPDATE Users " +
                     "SET Name = @UserName, " +
                     "AddressId = @UserAddressId, " +
                     "PhoneId = @UserPhoneId, " +
                     "OfficePhoneId = @UserOfficePhoneId, " +
                     "Deleted = @UserDeleted, " +
                     "UpdatedAt = @UserUpdatedAt " +
                     "WHERE Users.Id = @UserId", sqlQuery.GetSql());
    }

    [Fact]
    public static void Update_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
        var sqlQuery = sqlGenerator.GetUpdate(new User());

        Assert.Equal("UPDATE \"Users\" " +
                     "SET \"Name\" = @UserName, " +
                     "\"AddressId\" = @UserAddressId, " +
                     "\"PhoneId\" = @UserPhoneId, " +
                     "\"OfficePhoneId\" = @UserOfficePhoneId, " +
                     "\"Deleted\" = @UserDeleted, " +
                     "\"UpdatedAt\" = @UserUpdatedAt " +
                     "WHERE \"Users\".\"Id\" = @UserId", sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkUpdate()
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
    public static void BulkUpdate_QuoMarks()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, true);
        var phones = new List<Phone>
        {
            new() { Id = 10, IsActive = true, PNumber = "111" },
            new() { Id = 10, IsActive = false, PNumber = "222" }
        };

        var sqlQuery = sqlGenerator.GetBulkUpdate(phones);

        Assert.Equal("UPDATE \"DAB\".\"Phones\" SET \"PNumber\" = @PNumber0, \"IsActive\" = @IsActive0, \"Deleted\" = @Deleted0 WHERE \"Id\" = @Id0; " +
                     "UPDATE \"DAB\".\"Phones\" SET \"PNumber\" = @PNumber1, \"IsActive\" = @IsActive1, \"Deleted\" = @Deleted1 WHERE \"Id\" = @Id1", sqlQuery.GetSql());
    }
}
