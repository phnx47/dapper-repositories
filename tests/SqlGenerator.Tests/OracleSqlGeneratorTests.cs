using System;
using System.Collections.Generic;
using System.Linq;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using TestClasses;
using Xunit;

namespace SqlGenerator.Tests;

public class OracleSqlGeneratorTests
{
    private const SqlProvider _sqlConnector = SqlProvider.Oracle;

    [Fact]
    public static void Count()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetCount(null);
        Assert.Equal("SELECT COUNT(*) FROM Users WHERE Users.Deleted != 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinct()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetCount(null, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT Users.AddressId) FROM Users WHERE Users.Deleted != 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void CountWithDistinctAndWhere()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetCount(x => x.PhoneId == 1, user => user.AddressId);
        Assert.Equal("SELECT COUNT(DISTINCT Users.AddressId) FROM Users WHERE (Users.PhoneId = :PhoneId_p0) AND Users.Deleted != 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void ChangeDate_Insert()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var user = new User { Name = "Dude" };
        userSqlGenerator.GetInsert(user);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public static void ChangeDate_Update()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var user = new User { Name = "Dude" };
        userSqlGenerator.GetUpdate(user);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public static void ExpressionArgumentException()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var isExceptions = false;

        try
        {
            var sumAr = new List<int> { 1, 2, 3 };
            userSqlGenerator.GetSelectAll(x => sumAr.All(z => x.Id == z), null);
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
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

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

            userSqlGenerator.GetSelectAll(
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
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);

        var i = 0;
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        while (i++ < 10)
        {
            userSqlGenerator.GetSelectAll(x => x.UpdatedAt >= today, null);
            userSqlGenerator.GetSelectAll(x => x.UpdatedAt < tomorrow, null);
            userSqlGenerator.GetSelectAll(x => x.UpdatedAt >= today && x.UpdatedAt < tomorrow, null);
        }

        Assert.False(false, "dual ExpressionNullablePerformance");
    }

    [Fact]
    public static void BoolFalseEqualNotPredicate()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.IsActive != true, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.True(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code FROM DAB.Phones WHERE DAB.Phones.IsActive != :IsActive_p0 FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BoolFalseEqualPredicate()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.IsActive == false, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.False(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code FROM DAB.Phones WHERE DAB.Phones.IsActive = :IsActive_p0 FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BoolFalsePredicate()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectFirst(x => !x.IsActive, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.False(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code FROM DAB.Phones WHERE DAB.Phones.IsActive = :IsActive_p0 FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BoolTruePredicate()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.IsActive, null);

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.True(Convert.ToBoolean(parameters["IsActive_p0"]));

        Assert.Equal(
            "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code FROM DAB.Phones WHERE DAB.Phones.IsActive = :IsActive_p0 FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkInsertMultiple()
    {
        ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetBulkInsert(new List<Address> { new Address(), new Address() });

        //Assert.Equal("INSERT INTO Addresses (Street, CityId) VALUES (:Street0, :CityId0),(:Street1, :CityId1)", sqlQuery.GetSql());
        Assert.Equal("INSERT INTO Addresses (Street, CityId) SELECT :Street0, :CityId0 FROM DUAL UNION ALL SELECT :Street1, :CityId1 FROM DUAL", sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkInsertOne()
    {
        ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetBulkInsert(new List<Address> { new Address() });

        Assert.Equal("INSERT INTO Addresses (Street, CityId) SELECT :Street0, :CityId0 FROM DUAL", sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkInsertOneKeyAsIdentity()
    {
        MicroOrmConfig.AllowKeyAsIdentity = true;
        ISqlGenerator<AddressKeyAsIdentity> userSqlGenerator = new SqlGenerator<AddressKeyAsIdentity>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetBulkInsert(new List<AddressKeyAsIdentity> { new AddressKeyAsIdentity() });

        Assert.Equal("INSERT INTO Addresses (Street, CityId) SELECT :Street0, :CityId0 FROM DUAL", sqlQuery.GetSql());
        MicroOrmConfig.AllowKeyAsIdentity = false;
    }

    [Fact]
    public static void BulkUpdate()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var phones = new List<Phone>
        {
            new Phone { Id = 10, IsActive = true, PNumber = "111" },
            new Phone { Id = 10, IsActive = false, PNumber = "222" }
        };

        var sqlQuery = userSqlGenerator.GetBulkUpdate(phones);

        Assert.Equal("MERGE INTO DAB.Phones DAB.Phones " +
                     "USING (" +
                     "SELECT :PNumber0 AS PNumber, :IsActive0 AS IsActive, :Id0 AS Id FROM DUAL " +
                     "UNION ALL " +
                     "SELECT :PNumber1 AS PNumber, :IsActive1 AS IsActive, :Id1 AS Id FROM DUAL" +
                     ") DAB.Phones_BULKUPDATE ON (DAB.Phones_BULKUPDATE.Id = DAB.Phones.Id) " +
                     "WHEN MATCHED THEN UPDATE SET PNumber = DAB.Phones_BULKUPDATE.PNumber ,IsActive = DAB.Phones_BULKUPDATE.IsActive", sqlQuery.GetSql());
    }

    [Fact]
    public static void BulkUpdateIgnoreOneOfKeys()
    {
        ISqlGenerator<Report> userSqlGenerator = new SqlGenerator<Report>(_sqlConnector, false);
        var reports = new List<Report>
        {
            new Report { Id = 10, AnotherId = 10, UserId = 22 },
            new Report { Id = 10, AnotherId = 10, UserId = 23 }
        };

        var sqlQuery = userSqlGenerator.GetBulkUpdate(reports);

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
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var ids = new List<int>();
        var sqlQuery = userSqlGenerator.GetSelectAll(x => ids.Contains(x.Id), null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE (Users.Id IN :Id_p0) AND Users.Deleted != 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void ContainsArrayPredicate()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var ids = new int[] { };
        var sqlQuery = userSqlGenerator.GetSelectAll(x => ids.Contains(x.Id), null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE (Users.Id IN :Id_p0) AND Users.Deleted != 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void NotContainsPredicate()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var ids = new List<int>();
        var sqlQuery = userSqlGenerator.GetSelectAll(x => !ids.Contains(x.Id), null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE (Users.Id NOT IN :Id_p0) AND Users.Deleted != 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void LogicalDeleteWithUpdatedAt()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User() { Id = 10 };
        var sqlQuery = userSqlGenerator.GetDelete(user);
        var sql = sqlQuery.GetSql();

        Assert.Equal("UPDATE Users SET Deleted = 1, UpdatedAt = :UpdatedAt WHERE Users.Id = :Id", sql);
    }

    [Fact]
    public static void LogicalleleteWithUpdatedAtWithPredicate()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User() { Id = 10 };
        var sqlQuery = userSqlGenerator.GetDelete(user);
        var sql = sqlQuery.GetSql();

        Assert.Equal("UPDATE Users SET Deleted = 1, UpdatedAt = :UpdatedAt WHERE Users.Id = :Id", sql);
    }

    [Fact]
    public static void Delete()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var phone = new Phone { Id = 10, Code = "ZZZ", IsActive = true, PNumber = "111" };
        var sqlQuery = userSqlGenerator.GetDelete(phone);

        Assert.Equal("DELETE FROM DAB.Phones WHERE DAB.Phones.Id = :Id", sqlQuery.GetSql());
    }

    [Fact]
    public static void LogicalDeleteEntity()
    {
        ISqlGenerator<Car> sqlGenerator = new SqlGenerator<Car>(_sqlConnector);
        var car = new Car() { Id = 10, Name = "LogicalDelete", UserId = 5 };

        var sqlQuery = sqlGenerator.GetDelete(car);
        var realSql = sqlQuery.GetSql();
        Assert.Equal("UPDATE Cars SET Status = -1 WHERE Cars.Id = :Id", realSql);
    }

    [Fact]
    public static void LogicalDeletePredicate()
    {
        ISqlGenerator<Car> sqlGenerator = new SqlGenerator<Car>(_sqlConnector);

        var sqlQuery = sqlGenerator.GetDelete(q => q.Id == 10);
        var realSql = sqlQuery.GetSql();

        Assert.Equal("UPDATE Cars SET Status = -1 WHERE Cars.Id = :Id_p0", realSql);
    }

    [Fact]
    public static void DeleteWithMultiplePredicate()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetDelete(x => x.IsActive && x.PNumber == "111");

        Assert.Equal("DELETE FROM DAB.Phones WHERE DAB.Phones.IsActive = :IsActive_p0 AND DAB.Phones.PNumber = :PNumber_p1", sqlQuery.GetSql());
    }

    [Fact]
    public static void DeleteWithSinglePredicate()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetDelete(x => x.IsActive);

        Assert.Equal("DELETE FROM DAB.Phones WHERE DAB.Phones.IsActive = :IsActive_p0", sqlQuery.GetSql());
    }

    [Fact]
    public static void InsertQuoMarks()
    {
        ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetInsert(new Address());

        Assert.Equal("INSERT INTO Addresses (Street, CityId) VALUES (:Street, :CityId) RETURNING Id INTO :newId", sqlQuery.GetSql());
    }

    [Fact]
    public void Insert_AllowKeyAsIdentity_QuoMarks()
    {
        MicroOrmConfig.AllowKeyAsIdentity = true;
        ISqlGenerator<AddressKeyAsIdentity> userSqlGenerator = new SqlGenerator<AddressKeyAsIdentity>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetInsert(new AddressKeyAsIdentity());

        Assert.Equal("INSERT INTO Addresses (Street, CityId) VALUES (:Street, :CityId) RETURNING Id INTO :newId", sqlQuery.GetSql());
        MicroOrmConfig.AllowKeyAsIdentity = false;
    }

    [Fact]
    public static void IsNull()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectAll(user => user.UpdatedAt == null, null);

        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users " +
            "WHERE (Users.UpdatedAt IS NULL) AND Users.Deleted != 1", sqlQuery.GetSql());
        Assert.DoesNotContain("== NULL", sqlQuery.GetSql());
    }

    [Fact]
    public static void JoinBracelets()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectAll(null, null, user => user.Cars);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                     "Cars_Id.Id, Cars_Id.Name, Cars_Id.Data, Cars_Id.UserId, Cars_Id.Status " +
                     "FROM Users LEFT JOIN Cars Cars_Id ON Users.Id = Cars_Id.UserId " +
                     "WHERE Users.Deleted != 1", sqlQuery.GetSql());
    }

    [Fact]
    public static void NavigationPredicate()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Phone.PNumber == "123", null, user => user.Phone);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                     "Phones_PhoneId.Id, Phones_PhoneId.PNumber, Phones_PhoneId.IsActive, Phones_PhoneId.Code " +
                     "FROM Users INNER JOIN DAB.Phones Phones_PhoneId ON Users.PhoneId = Phones_PhoneId.Id " +
                     "WHERE (Phones_PhoneId.PNumber = :PhonePNumber_p0) AND Users.Deleted != 1", sqlQuery.GetSql());
    }


    [Fact]
    public static void SelectBetweenWithLogicalDelete()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, null, x => x.Id);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users " +
                     "WHERE Users.Deleted != 1 AND Users.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectBetweenWithoutLogicalDelete()
    {
        ISqlGenerator<Address> userSqlGenerator = new SqlGenerator<Address>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectBetween(1, 10, null, x => x.Id);

        Assert.Equal("SELECT Addresses.Id, Addresses.Street, Addresses.CityId FROM Addresses " +
                     "WHERE Addresses.Id BETWEEN '1' AND '10'", sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectById()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectById(1, null);

        Assert.Equal("SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt " +
                     "FROM Users WHERE Users.Id = :Id AND Users.Deleted != 1 FETCH FIRST 1 ROWS ONLY", sqlQuery.GetSql());
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
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 2, null);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted != 1 FETCH FIRST 1 ROW ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectLimit()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var filterData = new FilterData();
        var data = filterData.LimitInfo ?? new LimitInfo();
        data.Limit = 10u;
        filterData.LimitInfo = data;

        var sqlQuery = userSqlGenerator.GetSelectAll(x => x.Id == 2, filterData);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted != 1 OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectOrderBy()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var filterData = new FilterData();

        var data = filterData.OrderInfo ?? new OrderInfo();
        data.Columns = new List<string> { "Id" };
        data.Direction = OrderInfo.SortDirection.ASC;
        filterData.OrderInfo = data;

        var sqlQuery = userSqlGenerator.GetSelectAll(x => x.Id == 2, filterData);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted != 1 ORDER BY Id ASC",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void SelectPaged()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var filterData = new FilterData();

        var data = filterData.OrderInfo ?? new OrderInfo();
        data.Columns = new List<string> { "Id" };
        data.Direction = OrderInfo.SortDirection.ASC;
        filterData.OrderInfo = data;

        var dataLimit = filterData.LimitInfo ?? new LimitInfo();
        dataLimit.Limit = 10u;
        dataLimit.Offset = 5u;
        filterData.LimitInfo = dataLimit;

        var sqlQuery = userSqlGenerator.GetSelectAll(x => x.Id == 2, filterData);
        Assert.Equal(
            "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt FROM Users WHERE (Users.Id = :Id_p0) AND Users.Deleted != 1 ORDER BY Id ASC OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateExclude()
    {
        ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var phone = new Phone { Id = 10, Code = "ZZZ", IsActive = true, PNumber = "111" };
        var sqlQuery = userSqlGenerator.GetUpdate(phone);

        Assert.Equal("UPDATE DAB.Phones SET DAB.Phones.PNumber = :PhonePNumber, DAB.Phones.IsActive = :PhoneIsActive WHERE DAB.Phones.Id = :PhoneId",
            sqlQuery.GetSql());
    }

    [Fact]
    public static void UpdateWithPredicate()
    {
        ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetUpdate(q => q.Identifier == Guid.Empty, new City());
        var sql = sqlQuery.GetSql();
        Assert.Equal("UPDATE Cities SET Cities.Identifier = :CityIdentifier, Cities.Name = :CityName WHERE Cities.Identifier = :Identifier_p0", sql);
    }

    [Fact]
    public static void SelectGroupConditionsWithPredicate()
    {
        ISqlGenerator<Phone> phoneSqlGenerator = new SqlGenerator<Phone>(_sqlConnector, false);
        var sPrefix = "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code FROM DAB.Phones WHERE ";

        var sqlQuery1 = phoneSqlGenerator.GetSelectAll(x => (x.IsActive && x.Id == 123) || (x.Id == 456 && x.PNumber == "456"), null);
        Assert.Equal(
            sPrefix + "(DAB.Phones.IsActive = :IsActive_p0 AND DAB.Phones.Id = :Id_p1) OR (DAB.Phones.Id = :Id_p2 AND DAB.Phones.PNumber = :PNumber_p3)",
            sqlQuery1.GetSql());

        var sqlQuery2 = phoneSqlGenerator.GetSelectAll(x => !x.IsActive || (x.Id == 456 && x.PNumber == "456"), null);
        Assert.Equal(sPrefix + "DAB.Phones.IsActive = :IsActive_p0 OR (DAB.Phones.Id = :Id_p1 AND DAB.Phones.PNumber = :PNumber_p2)", sqlQuery2.GetSql());

        var sqlQuery3 = phoneSqlGenerator.GetSelectAll(x => (x.Id == 456 && x.PNumber == "456") || x.Id == 123, null);
        Assert.Equal(sPrefix + "(DAB.Phones.Id = :Id_p0 AND DAB.Phones.PNumber = :PNumber_p1) OR DAB.Phones.Id = :Id_p2", sqlQuery3.GetSql());

        var sqlQuery4 = phoneSqlGenerator.GetSelectAll(x => x.PNumber == "1" && (x.IsActive || x.PNumber == "456") && x.Id == 123, null);
        Assert.Equal(
            sPrefix +
            "DAB.Phones.PNumber = :PNumber_p0 AND (DAB.Phones.IsActive = :IsActive_p1 OR DAB.Phones.PNumber = :PNumber_p2) AND DAB.Phones.Id = :Id_p3",
            sqlQuery4.GetSql());

        var sqlQuery5 = phoneSqlGenerator.GetSelectAll(x => x.PNumber == "1" && (x.IsActive || x.PNumber == "456" || x.PNumber == "678") && x.Id == 123, null);
        Assert.Equal(
            sPrefix +
            "DAB.Phones.PNumber = :PNumber_p0 AND (DAB.Phones.IsActive = :IsActive_p1 OR DAB.Phones.PNumber = :PNumber_p2 OR DAB.Phones.PNumber = :PNumber_p3) AND DAB.Phones.Id = :Id_p4",
            sqlQuery5.GetSql());

        var ids = new List<int>();
        var sqlQuery6 = phoneSqlGenerator.GetSelectAll(x => !x.IsActive || (x.IsActive && ids.Contains(x.Id)), null);
        Assert.Equal(sPrefix + "DAB.Phones.IsActive = :IsActive_p0 OR (DAB.Phones.IsActive = :IsActive_p1 AND DAB.Phones.Id IN :Id_p2)", sqlQuery6.GetSql());

        var sqlQuery7 = phoneSqlGenerator.GetSelectAll(x => (x.IsActive && x.Id == 123) && (x.Id == 456 && x.PNumber == "456"), null);
        Assert.Equal(
            sPrefix + "DAB.Phones.IsActive = :IsActive_p0 AND DAB.Phones.Id = :Id_p1 AND DAB.Phones.Id = :Id_p2 AND DAB.Phones.PNumber = :PNumber_p3",
            sqlQuery7.GetSql());

        var sqlQuery8 = phoneSqlGenerator.GetSelectAll(
            x => x.PNumber == "1" && (x.IsActive || x.PNumber == "456" || x.PNumber == "123" || (x.Id == 1213 && x.PNumber == "678")) && x.Id == 123, null);
        Assert.Equal(
            sPrefix +
            "DAB.Phones.PNumber = :PNumber_p0 AND (DAB.Phones.IsActive = :IsActive_p1 OR DAB.Phones.PNumber = :PNumber_p2 OR DAB.Phones.PNumber = :PNumber_p3 OR (DAB.Phones.Id = :Id_p4 AND DAB.Phones.PNumber = :PNumber_p5)) AND DAB.Phones.Id = :Id_p6",
            sqlQuery8.GetSql());

        var sqlQuery9 = phoneSqlGenerator.GetSelectAll(x => (x.Id == 456 && x.PNumber == "456") && x.Id == 123 && (x.Id == 4567 && x.PNumber == "4567"), null);
        Assert.Equal(
            sPrefix +
            "DAB.Phones.Id = :Id_p0 AND DAB.Phones.PNumber = :PNumber_p1 AND DAB.Phones.Id = :Id_p2 AND DAB.Phones.Id = :Id_p3 AND DAB.Phones.PNumber = :PNumber_p4",
            sqlQuery9.GetSql());
    }

    [Fact]
    public static void SelectGroupConditionsNavigationPredicate()
    {
        ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, false);
        var sPrefix = "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                      "Phones_PhoneId.Id, Phones_PhoneId.PNumber, Phones_PhoneId.IsActive, Phones_PhoneId.Code " +
                      "FROM Users INNER JOIN DAB.Phones Phones_PhoneId ON Users.PhoneId = Phones_PhoneId.Id " +
                      "WHERE ";

        var sqlQuery1 = userSqlGenerator.GetSelectFirst(x => x.Phone.PNumber == "123" || (x.Name == "abc" && x.Phone.IsActive), null, user => user.Phone);
        Assert.Equal(
            sPrefix +
            "(Phones_PhoneId.PNumber = :PhonePNumber_p0 OR (Users.Name = :Name_p1 AND Phones_PhoneId.IsActive = :PhoneIsActive_p2)) AND Users.Deleted != 1",
            sqlQuery1.GetSql());

        var ids = new List<int>();
        var sqlQuery2 = userSqlGenerator.GetSelectFirst(
            x => x.Phone.PNumber != "123" && (x.Name != "abc" || !x.Phone.IsActive || !ids.Contains(x.PhoneId) || !ids.Contains(x.Phone.Id)) &&
                 (x.Name == "abc" || x.Phone.IsActive), null, user => user.Phone);
        Assert.Equal(
            sPrefix +
            "(Phones_PhoneId.PNumber != :PhonePNumber_p0 AND (Users.Name != :Name_p1 OR Phones_PhoneId.IsActive = :PhoneIsActive_p2 OR Users.PhoneId NOT IN :PhoneId_p3 OR Phones_PhoneId.Id NOT IN :PhoneId_p4) AND (Users.Name = :Name_p5 OR Phones_PhoneId.IsActive = :PhoneIsActive_p6)) AND Users.Deleted != 1",
            sqlQuery2.GetSql());
    }

    [Fact]
    public static void SelectLikeWithPredicate()
    {
        ISqlGenerator<Phone> phoneSqlGenerator1 = new SqlGenerator<Phone>(_sqlConnector, false);
        var sPrefix1 = "SELECT DAB.Phones.Id, DAB.Phones.PNumber, DAB.Phones.IsActive, DAB.Phones.Code FROM DAB.Phones WHERE ";

        var sqlQuery11 = phoneSqlGenerator1.GetSelectAll(x => x.Code.StartsWith("123", StringComparison.OrdinalIgnoreCase) || !x.Code.EndsWith("456") || x.Code.Contains("789"),
            null);
        Assert.Equal(sPrefix1 + "DAB.Phones.Code LIKE :Code_p0 OR DAB.Phones.Code NOT LIKE :Code_p1 OR DAB.Phones.Code LIKE :Code_p2", sqlQuery11.GetSql());

        var parameters11 = sqlQuery11.Param as IDictionary<string, object>;
        Assert.True("123%" == parameters11["Code_p0"].ToString());
        Assert.True("%456" == parameters11["Code_p1"].ToString());
        Assert.True("%789%" == parameters11["Code_p2"].ToString());

        ISqlGenerator<User> userSqlGenerator2 = new SqlGenerator<User>(_sqlConnector, false);
        var sPrefix2 = "SELECT Users.Id, Users.Name, Users.AddressId, Users.PhoneId, Users.OfficePhoneId, Users.Deleted, Users.UpdatedAt, " +
                       "Phones_PhoneId.Id, Phones_PhoneId.PNumber, Phones_PhoneId.IsActive, Phones_PhoneId.Code " +
                       "FROM Users INNER JOIN DAB.Phones Phones_PhoneId ON Users.PhoneId = Phones_PhoneId.Id " +
                       "WHERE ";

        var sqlQuery21 = userSqlGenerator2.GetSelectFirst(x => x.Phone.PNumber.StartsWith("123") || (!x.Name.Contains("abc") && x.Phone.IsActive), null, user => user.Phone);
        Assert.Equal(
            sPrefix2 +
            "(Phones_PhoneId.PNumber LIKE :PhonePNumber_p0 OR (Users.Name NOT LIKE :Name_p1 AND Phones_PhoneId.IsActive = :PhoneIsActive_p2)) AND Users.Deleted != 1",
            sqlQuery21.GetSql());
        var parameters21 = sqlQuery21.Param as IDictionary<string, object>;
        Assert.True("123%" == parameters21["PhonePNumber_p0"].ToString());
        Assert.True("%abc%" == parameters21["Name_p1"].ToString());
    }
}
