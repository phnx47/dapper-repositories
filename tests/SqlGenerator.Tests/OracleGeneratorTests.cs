using System;
using System.Collections.Generic;
using System.Linq;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using TestClasses;
using Xunit;

namespace SqlGenerator.Tests;

public class OracleGeneratorTests
{
    private const SqlProvider _sqlConnector = SqlProvider.Oracle;

    [Fact]
    public static void Count()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetCount(null);
        Assert.Equal("SELECT COUNT(*) FROM Users WHERE Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinct()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetCount(null, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT Users.AddressId) FROM Users WHERE Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinctAndWhere()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetCount(x => x.PhoneId == 1, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT Users.AddressId) FROM Users WHERE (Users.PhoneId = :PhoneId_p0) AND Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void ChangeDate_Insert()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var user = new User { Name = "Dude" };
        sqlGenerator.GetInsert(user);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public static void ChangeDate_Update()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var user = new User { Name = "Dude" };
        sqlGenerator.GetUpdate(user);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public static void ExpressionArgumentException()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var isExceptions = false;

        try
        {
            var sumAr = new List<int> { 1, 2, 3 };
            sqlGenerator.GetSelectAll(x => sumAr.All(z => x.Id == z), null);
        }
        catch (NotSupportedException ex)
        {
            Assert.Contains("'All' method is not supported", ex.Message);
            isExceptions = true;
        }

        Assert.True(isExceptions, "Contains no cast exception");
    }

    [Fact]
    public static void ExpressionComplicatedCollectionContains()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var isExceptions = false;

        try
        {
            var tmp = new ComplicatedObj();

            var id = 1;
            var ids = new List<int> { 1, 2, 3 };
            var farIds = tmp.FieldArIds;
            var pNames = tmp.PropertyNames;

            var sfarIds = ComplicatedObj.StaticFieldArIds;
            var spNames = ComplicatedObj.StaticPropertyNames;

            sqlGenerator.GetSelectAll(
                x => (
                    (ids.Contains(x.Id) || farIds.Contains(x.Id) || sfarIds.Contains(x.Id)
                     || tmp.FieldArIds.Contains(x.Id) || x.Id == tmp.Id
                     || ComplicatedObj.StaticFieldArIds.Contains(x.Id)
                     || x.Id == id)
                    && (pNames.Contains(x.Name) || spNames.Contains(x.Name)
                                                || tmp.PropertyNames.Contains(x.Name) || tmp.FieldNames.Contains(x.Name)
                                                || ComplicatedObj.StaticFieldNames.Contains(x.Name)
                                                || ComplicatedObj.StaticPropertyNames.Contains(x.Name)
                                                || x.Name == ComplicatedObj.StaticName
                                                || x.Name == ComplicatedObj.StaticPropertyName
                                                || x.Name == tmp.PropertName
                                                || x.Name == Guid.NewGuid().ToString()
                                                || x.Name == string.Empty)
                ), null);
        }
        catch (NotSupportedException ex)
        {
            Assert.Contains("isn't supported", ex.Message);
            isExceptions = true;
        }

        Assert.False(isExceptions, "Complicated_Collection_Contains MemberAccess exception");
    }

    [Fact]
    public static void ExpressionNullablePerformance()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var i = 0;
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        while (i++ < 10)
        {
            sqlGenerator.GetSelectAll(x => x.UpdatedAt >= today, null);
            sqlGenerator.GetSelectAll(x => x.UpdatedAt < tomorrow, null);
            sqlGenerator.GetSelectAll(x => x.UpdatedAt >= today && x.UpdatedAt < tomorrow, null);
        }

        Assert.False(false, "dual ExpressionNullablePerformance");
    }

    [Fact]
    public static void BoolFalseEqualNotPredicate()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.IsActive != true, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.True(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code, DAB.Phones.Deleted FROM DAB.Phones WHERE (DAB.Phones.IsActive != :IsActive_p0) AND DAB.Phones.Deleted IS NULL FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BoolFalseEqualPredicate()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.IsActive == false, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.False(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code, DAB.Phones.Deleted FROM DAB.Phones WHERE (DAB.Phones.IsActive = :IsActive_p0) AND DAB.Phones.Deleted IS NULL FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BoolFalsePredicate()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => !x.IsActive, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.False(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code, DAB.Phones.Deleted FROM DAB.Phones WHERE (DAB.Phones.IsActive = :IsActive_p0) AND DAB.Phones.Deleted IS NULL FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BoolTruePredicate()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.IsActive, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.True(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code, DAB.Phones.Deleted FROM DAB.Phones WHERE (DAB.Phones.IsActive = :IsActive_p0) AND DAB.Phones.Deleted IS NULL FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkInsertMultiple()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetBulkInsert(new List<Address> { new(), new() });

        Assert.Equal("INSERT INTO Addresses (Street, CityId) SELECT :Street0, :CityId0 FROM DUAL UNION ALL SELECT :Street1, :CityId1 FROM DUAL",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkInsertOne()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetBulkInsert(new List<Address> { new() });

        Assert.Equal("INSERT INTO Addresses (Street, CityId) SELECT :Street0, :CityId0 FROM DUAL", sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkInsertOneKeyAsIdentity()
    {
        MicroOrmConfig.AllowKeyAsIdentity = true;
        var sqlGenerator = new SqlGenerator<AddressKeyAsIdentity>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetBulkInsert(new List<AddressKeyAsIdentity> { new() });

        Assert.Equal("INSERT INTO Addresses (Street, CityId) SELECT :Street0, :CityId0 FROM DUAL", sqlQuery.GetSql());
        MicroOrmConfig.AllowKeyAsIdentity = false;
    }

    [Fact]
    public static void BulkUpdate()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var phones = new List<Phone>
        {
            new() { Id = 10, IsActive = true, PNumber = "111" },
            new() { Id = 10, IsActive = false, PNumber = "222" }
        };

        var sqlQuery = sqlGenerator.GetBulkUpdate(phones);

        Assert.Equal("MERGE INTO DAB.Phones DAB.Phones " +
                     "USING (" +
                     "SELECT :PNumber0 AS PNumber, :IsActive0 AS IsActive, :Deleted0 AS Deleted, :Id0 AS Id FROM DUAL " +
                     "UNION ALL " +
                     "SELECT :PNumber1 AS PNumber, :IsActive1 AS IsActive, :Deleted1 AS Deleted, :Id1 AS Id FROM DUAL" +
                     ") DAB.Phones_BULKUPDATE ON (DAB.Phones_BULKUPDATE.Id = DAB.Phones.Id) " +
                     "WHEN MATCHED THEN UPDATE SET PNumber = DAB.Phones_BULKUPDATE.PNumber ,IsActive = DAB.Phones_BULKUPDATE.IsActive ,Deleted = DAB.Phones_BULKUPDATE.Deleted", sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkUpdateIgnoreOneOfKeys()
    {
        var sqlGenerator = new SqlGenerator<Report>(_sqlConnector, false);
        var reports = new List<Report>
        {
            new() { Id = 10, AnotherId = 10, UserId = 22 },
            new() { Id = 10, AnotherId = 10, UserId = 23 }
        };

        var sqlQuery = sqlGenerator.GetBulkUpdate(reports);

        Assert.Equal("MERGE INTO Reports Reports " +
                     "USING (" +
                     "SELECT :UserId0 AS UserId, :AnotherId0 AS AnotherId FROM DUAL " +
                     "UNION ALL " +
                     "SELECT :UserId1 AS UserId, :AnotherId1 AS AnotherId FROM DUAL" +
                     ") Reports_BULKUPDATE ON (Reports_BULKUPDATE.AnotherId = Reports.AnotherId) " +
                     "WHEN MATCHED THEN UPDATE SET UserId = Reports_BULKUPDATE.UserId", sqlQuery.GetSql());
    }

    [Fact]
    public static void ContainsPredicate()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var ids = new List<int>();
        var sqlQuery = sqlGenerator.GetSelectAll(x => ids.Contains(x.Id), null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE (Users.Id IN :Id_p0) AND Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void ContainsArrayPredicate()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var ids = Array.Empty<int>();
        var sqlQuery = sqlGenerator.GetSelectAll(x => ids.Contains(x.Id), null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE (Users.Id IN :Id_p0) AND Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void NotContainsPredicate()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var ids = new List<int>();
        var sqlQuery = sqlGenerator.GetSelectAll(x => !ids.Contains(x.Id), null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE (Users.Id NOT IN :Id_p0) AND Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void LogicalDeleteWithUpdatedAt()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Id = 10 };
        var sqlQuery = sqlGenerator.GetDelete(user);
        var sql = sqlQuery.GetSql();

        Assert.Equal("UPDATE Users SET Deleted = 1, UpdatedAt = :UpdatedAt WHERE Users.Id = :Id", sql);
    }

    [Fact]
    public static void Delete()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector);
        var address = new Address { Street = "aaa0", CityId = Guid.NewGuid().ToString() };
        var sql = sqlGenerator.GetDelete(address).GetSql();
        Assert.Equal("DELETE FROM Addresses WHERE Addresses.Id = :Id", sql);
    }

    [Fact]
    public static void DeleteWithSinglePredicate()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector);
        var sql = sqlGenerator.GetDelete(x => x.CityId == "2ed78b44-2fea-4a00-b507-e147d7b4a018").GetSql();

        Assert.Equal("DELETE FROM Addresses WHERE Addresses.CityId = :CityId_p0", sql);
    }

    [Fact]
    public static void DeleteWithMultiplePredicate()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector);
        var sql = sqlGenerator.GetDelete(x => x.CityId == "2ed78b44-2fea-4a00-b507-e147d7b4a018" && x.Street == "aaa1").GetSql();

        Assert.Equal("DELETE FROM Addresses WHERE Addresses.CityId = :CityId_p0 AND Addresses.Street = :Street_p1", sql);
    }

    [Fact]
    public static void LogicalDeleteEntity()
    {
        var sqlGenerator = new SqlGenerator<Car>(_sqlConnector);
        var car = new Car { Id = 10, Name = "LogicalDelete", UserId = 5 };

        var sqlQuery = sqlGenerator.GetDelete(car);
        var realSql = sqlQuery.GetSql();
        Assert.Equal("UPDATE Cars SET Status = -1 WHERE Cars.Id = :Id", realSql);
    }

    [Fact]
    public static void LogicalDeletePredicate()
    {
        var sqlGenerator = new SqlGenerator<Car>(_sqlConnector);

        var sqlQuery = sqlGenerator.GetDelete(q => q.Id == 10);
        var realSql = sqlQuery.GetSql();

        Assert.Equal("UPDATE Cars SET Status = -1 WHERE Cars.Id = :Id_p0", realSql);
    }

    [Fact]
    public static void InsertQuoMarks()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetInsert(new Address());

        Assert.Equal("INSERT INTO Addresses (Street, CityId) VALUES (:Street, :CityId) RETURNING Id INTO :newId", sqlQuery.GetSql());
    }

    [Fact]
    public void Insert_AllowKeyAsIdentity_QuoMarks()
    {
        MicroOrmConfig.AllowKeyAsIdentity = true;
        var sqlGenerator = new SqlGenerator<AddressKeyAsIdentity>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetInsert(new AddressKeyAsIdentity());

        Assert.Equal("INSERT INTO Addresses (Street, CityId) VALUES (:Street, :CityId) RETURNING Id INTO :newId", sqlQuery.GetSql());
        MicroOrmConfig.AllowKeyAsIdentity = false;
    }

    [Fact]
    public static void IsNull()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectAll(user => user.UpdatedAt == null, null);

        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users " +
            "WHERE (Users.UpdatedAt IS NULL) AND Users.Deleted IS NULL", sqlQuery.GetSql());
        Assert.DoesNotContain("== NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void JoinBracelets()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectAll(null, null, user => user.Cars);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                     "Cars_Id.Id, Cars_Id.Name, Cars_Id.Data, Cars_Id.UserId, Cars_Id.Status " +
                     "FROM Users LEFT JOIN Cars Cars_Id ON Users.Id = Cars_Id.UserId " +
                     "WHERE Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void NavigationPredicate()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Phone.PNumber == "123", null, user => user.Phone);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                     "Phones_PhoneId.Id, Phones_PhoneId.PNumber, Phones_PhoneId.IsActive, Phones_PhoneId.Code, Phones_PhoneId.Deleted " +
                     "FROM Users INNER JOIN DAB.Phones Phones_PhoneId ON Users.PhoneId = Phones_PhoneId.Id " +
                     "WHERE (Phones_PhoneId.PNumber = :PhonePNumber_p0) AND Users.Deleted IS NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectBetweenWithLogicalDelete()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectBetween(1, 10, null, x => x.Id);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users " +
                     "WHERE Users.Deleted IS NULL AND Users.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectBetweenWithoutLogicalDelete()
    {
        var sqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectBetween(1, 10, null, x => x.Id);

        Assert.Equal("SELECT Addresses.Id, Addresses.Street, Addresses.CityId FROM Addresses " +
                     "WHERE Addresses.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectById()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectById(1, null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE Users.Id = :Id AND Users.Deleted IS NULL FETCH FIRST 1 ROWS ONLY", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectByIdJoin()
    {
        var generator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = generator.GetSelectById(1, null, q => q.Users);
        Assert.Equal("SELECT Addresses.Id, Addresses.Street, Addresses.CityId, Users.Id, Users.Name, Users.AddressId, Users.PhoneId, " +
                     "Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Addresses " +
                     "LEFT JOIN Users ON Addresses.Id = Users.AddressId WHERE Addresses.Id = :Id", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectFirst()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Id == 2, null);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted IS NULL FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectLimit()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var filterData = new FilterData();
        var data = filterData.LimitInfo ?? new LimitInfo();
        data.Limit = 10u;
        filterData.LimitInfo = data;

        var sqlQuery = sqlGenerator.GetSelectAll(x => x.Id == 2, filterData);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted IS NULL OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectOrderBy()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var filterData = new FilterData();

        var data = filterData.OrderInfo ?? new OrderInfo();
        data.Columns = ["Id"];
        data.Direction = OrderInfo.SortDirection.ASC;
        filterData.OrderInfo = data;

        var sqlQuery = sqlGenerator.GetSelectAll(x => x.Id == 2, filterData);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted IS NULL ORDER BY Id ASC",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectPaged()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var filterData = new FilterData();

        var data = filterData.OrderInfo ?? new OrderInfo();
        data.Columns = new List<string> { "Id" };
        data.Direction = OrderInfo.SortDirection.ASC;
        filterData.OrderInfo = data;

        var dataLimit = filterData.LimitInfo ?? new LimitInfo();
        dataLimit.Limit = 10u;
        dataLimit.Offset = 5u;
        filterData.LimitInfo = dataLimit;

        var sqlQuery = sqlGenerator.GetSelectAll(x => x.Id == 2, filterData);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted IS NULL ORDER BY Id ASC OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateExclude()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var phone = new Phone { Id = 10, Code = "ZZZ", IsActive = true, PNumber = "111" };
        var sqlQuery = sqlGenerator.GetUpdate(phone);

        Assert.Equal("UPDATE DAB.Phones SET DAB.Phones.PNumber = :PhonePNumber, DAB.Phones.IsActive = :PhoneIsActive, DAB.Phones.Deleted = :PhoneDeleted WHERE DAB.Phones.Id = :PhoneId",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateWithPredicate()
    {
        var sqlGenerator = new SqlGenerator<City>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetUpdate(q => q.Identifier == Guid.Empty, new City());
        var sql = sqlQuery.GetSql();
        Assert.Equal("UPDATE Cities SET Cities.Identifier = :CityIdentifier, Cities.Name = :CityName WHERE Cities.Identifier = :Identifier_p0", sql);
    }

    [Fact]
    public static void SelectGroupConditionsWithPredicate()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        const string sPrefix = "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code, DAB.Phones.Deleted FROM DAB.Phones WHERE ";

        var sqlQuery1 = sqlGenerator.GetSelectAll(x => (x.IsActive && x.Id == 123) || (x.Id == 456 && x.PNumber == "456"), null);
        Assert.Equal(
            sPrefix + "((DAB.Phones.IsActive = :IsActive_p0 AND DAB.Phones.Id = :Id_p1) OR (DAB.Phones.Id = :Id_p2 AND DAB.Phones.PNumber = :PNumber_p3)) AND DAB.Phones.Deleted IS NULL",
            sqlQuery1.GetSql());

        var sqlQuery2 = sqlGenerator.GetSelectAll(x => !x.IsActive || (x.Id == 456 && x.PNumber == "456"), null);
        Assert.Equal(sPrefix + "(DAB.Phones.IsActive = :IsActive_p0 OR (DAB.Phones.Id = :Id_p1 AND DAB.Phones.PNumber = :PNumber_p2)) AND DAB.Phones.Deleted IS NULL", sqlQuery2.GetSql());

        var sqlQuery3 = sqlGenerator.GetSelectAll(x => (x.Id == 456 && x.PNumber == "456") || x.Id == 123, null);
        Assert.Equal(sPrefix + "((DAB.Phones.Id = :Id_p0 AND DAB.Phones.PNumber = :PNumber_p1) OR DAB.Phones.Id = :Id_p2) AND DAB.Phones.Deleted IS NULL", sqlQuery3.GetSql());

        var sqlQuery4 = sqlGenerator.GetSelectAll(x => x.PNumber == "1" && (x.IsActive || x.PNumber == "456") && x.Id == 123, null);
        Assert.Equal(
            sPrefix +
            "(DAB.Phones.PNumber = :PNumber_p0 AND (DAB.Phones.IsActive = :IsActive_p1 OR DAB.Phones.PNumber = :PNumber_p2) AND DAB.Phones.Id = :Id_p3) AND DAB.Phones.Deleted IS NULL",
            sqlQuery4.GetSql());

        var sqlQuery5 = sqlGenerator.GetSelectAll(x => x.PNumber == "1" && (x.IsActive || x.PNumber == "456" || x.PNumber == "678") && x.Id == 123, null);
        Assert.Equal(
            sPrefix +
            "(DAB.Phones.PNumber = :PNumber_p0 AND (DAB.Phones.IsActive = :IsActive_p1 OR DAB.Phones.PNumber = :PNumber_p2 OR DAB.Phones.PNumber = :PNumber_p3) AND DAB.Phones.Id = :Id_p4) AND DAB.Phones.Deleted IS NULL",
            sqlQuery5.GetSql());

        var ids = new List<int>();
        var sqlQuery6 = sqlGenerator.GetSelectAll(x => !x.IsActive || (x.IsActive && ids.Contains(x.Id)), null);
        Assert.Equal(sPrefix + "(DAB.Phones.IsActive = :IsActive_p0 OR (DAB.Phones.IsActive = :IsActive_p1 AND DAB.Phones.Id IN :Id_p2)) AND DAB.Phones.Deleted IS NULL", sqlQuery6.GetSql());

        var sqlQuery7 = sqlGenerator.GetSelectAll(x => (x.IsActive && x.Id == 123) && (x.Id == 456 && x.PNumber == "456"), null);
        Assert.Equal(
            sPrefix + "(DAB.Phones.IsActive = :IsActive_p0 AND DAB.Phones.Id = :Id_p1 AND DAB.Phones.Id = :Id_p2 AND DAB.Phones.PNumber = :PNumber_p3) AND DAB.Phones.Deleted IS NULL",
            sqlQuery7.GetSql());

        var sqlQuery8 = sqlGenerator.GetSelectAll(
            x => x.PNumber == "1" && (x.IsActive || x.PNumber == "456" || x.PNumber == "123" || (x.Id == 1213 && x.PNumber == "678")) && x.Id == 123, null);
        Assert.Equal(
            sPrefix +
            "(DAB.Phones.PNumber = :PNumber_p0 AND (DAB.Phones.IsActive = :IsActive_p1 OR DAB.Phones.PNumber = :PNumber_p2 OR DAB.Phones.PNumber = :PNumber_p3 OR (DAB.Phones.Id = :Id_p4 AND DAB.Phones.PNumber = :PNumber_p5)) AND DAB.Phones.Id = :Id_p6) AND DAB.Phones.Deleted IS NULL",
            sqlQuery8.GetSql());

        var sqlQuery9 = sqlGenerator.GetSelectAll(x => (x.Id == 456 && x.PNumber == "456") && x.Id == 123 && (x.Id == 4567 && x.PNumber == "4567"), null);
        Assert.Equal(
            sPrefix +
            "(DAB.Phones.Id = :Id_p0 AND DAB.Phones.PNumber = :PNumber_p1 AND DAB.Phones.Id = :Id_p2 AND DAB.Phones.Id = :Id_p3 AND DAB.Phones.PNumber = :PNumber_p4) AND DAB.Phones.Deleted IS NULL",
            sqlQuery9.GetSql());
    }

    [Fact]
    public static void SelectGroupConditionsNavigationPredicate()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        const string sPrefix = "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                               "Phones_PhoneId.Id, Phones_PhoneId.PNumber, Phones_PhoneId.IsActive, Phones_PhoneId.Code, Phones_PhoneId.Deleted " +
                               "FROM Users INNER JOIN DAB.Phones Phones_PhoneId ON Users.PhoneId = Phones_PhoneId.Id " +
                               "WHERE ";

        var sqlQuery1 = sqlGenerator.GetSelectFirst(x => x.Phone.PNumber == "123" || (x.Name == "abc" && x.Phone.IsActive), null, user => user.Phone);
        Assert.Equal(
            sPrefix +
            "(Phones_PhoneId.PNumber = :PhonePNumber_p0 OR (Users.Name = :Name_p1 AND Phones_PhoneId.IsActive = :PhoneIsActive_p2)) AND Users.Deleted IS NULL",
            sqlQuery1.GetSql());

        var ids = new List<int>();
        var sqlQuery2 = sqlGenerator.GetSelectFirst(
            x => x.Phone.PNumber != "123" && (x.Name != "abc" || !x.Phone.IsActive || !ids.Contains(x.PhoneId) || !ids.Contains(x.Phone.Id)) &&
                 (x.Name == "abc" || x.Phone.IsActive), null, user => user.Phone);
        Assert.Equal(
            sPrefix +
            "(Phones_PhoneId.PNumber != :PhonePNumber_p0 AND (Users.Name != :Name_p1 OR Phones_PhoneId.IsActive = :PhoneIsActive_p2 OR Users.PhoneId NOT IN :PhoneId_p3 OR Phones_PhoneId.Id NOT IN :PhoneId_p4) AND (Users.Name = :Name_p5 OR Phones_PhoneId.IsActive = :PhoneIsActive_p6)) AND Users.Deleted IS NULL",
            sqlQuery2.GetSql());
    }

    [Fact]
    public static void SelectLikeWithPredicate()
    {
        var phoneSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        const string sPrefix1 = "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code, DAB.Phones.Deleted FROM DAB.Phones WHERE ";

        var sqlQuery11 = phoneSqlGenerator.GetSelectAll(
            x => x.Code.StartsWith("123", StringComparison.OrdinalIgnoreCase) || !x.Code.EndsWith("456") || x.Code.Contains("789"),
            null);
        Assert.Equal(
            sPrefix1 +
            "(DAB.Phones.Code LIKE :Code_p0 OR DAB.Phones.Code NOT LIKE :Code_p1 OR DAB.Phones.Code LIKE :Code_p2) AND DAB.Phones.Deleted IS NULL",
            sqlQuery11.GetSql());

        var parameters11 = sqlQuery11.Param as IDictionary<string, object>;
        Assert.Equal("123%", parameters11["Code_p0"].ToString());
        Assert.Equal("%456", parameters11["Code_p1"].ToString());
        Assert.Equal("%789%", parameters11["Code_p2"].ToString());

        var userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sPrefix2 = "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                       "Phones_PhoneId.Id, Phones_PhoneId.PNumber, Phones_PhoneId.IsActive, Phones_PhoneId.Code, Phones_PhoneId.Deleted " +
                       "FROM Users INNER JOIN DAB.Phones Phones_PhoneId ON Users.PhoneId = Phones_PhoneId.Id " +
                       "WHERE ";

        var sqlQuery21 = userSqlGenerator.GetSelectFirst(x => x.Phone.PNumber.StartsWith("123") || (!x.Name.Contains("abc") && x.Phone.IsActive), null,
            user => user.Phone);
        Assert.Equal(
            sPrefix2 +
            "(Phones_PhoneId.PNumber LIKE :PhonePNumber_p0 OR (Users.Name NOT LIKE :Name_p1 AND Phones_PhoneId.IsActive = :PhoneIsActive_p2)) AND Users.Deleted IS NULL",
            sqlQuery21.GetSql());
        var parameters21 = sqlQuery21.Param as IDictionary<string, object>;
        Assert.Equal("123%", parameters21["PhonePNumber_p0"].ToString());
        Assert.Equal("%abc%", parameters21["Name_p1"].ToString());
    }
}
