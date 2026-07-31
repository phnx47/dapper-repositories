using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using TestClasses;
using Xunit;

namespace SqlGenerator.Tests;

public class UpdatedAtTests
{
    private const SqlProvider _sqlConnector = SqlProvider.MSSQL;

    [Fact]
    public static void InsertSetsUtcNow()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Name = "John" };

        var before = DateTime.UtcNow;
        sqlGenerator.GetInsert(user);

        Assert.InRange(user.UpdatedAt.Value, before, DateTime.UtcNow);
    }

    [Fact]
    public static void UpdateSetsUtcNow()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Id = 10, Name = "John" };

        var before = DateTime.UtcNow;
        sqlGenerator.GetUpdate(user);

        Assert.InRange(user.UpdatedAt.Value, before, DateTime.UtcNow);
    }

    [Fact]
    public static void LogicalDeleteSetsUtcNow()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Id = 10 };

        var before = DateTime.UtcNow;
        sqlGenerator.GetDelete(user);

        Assert.InRange(user.UpdatedAt.Value, before, DateTime.UtcNow);
    }

    [Fact]
    public static void LocalTimeKindSetsLocalNow()
    {
        var sqlGenerator = new SqlGenerator<UpdatedAtLocal>(_sqlConnector);
        var entity = new UpdatedAtLocal { Id = 10, Name = "John" };

        var before = DateTime.Now;
        sqlGenerator.GetUpdate(entity);

        Assert.InRange(entity.UpdatedAt.Value, before, DateTime.Now);
    }

    [Fact]
    public static void OffSetShiftsTheValue()
    {
        var sqlGenerator = new SqlGenerator<UpdatedAtOffSet>(_sqlConnector);
        var entity = new UpdatedAtOffSet { Id = 10, Name = "John" };

        var before = DateTime.UtcNow.AddHours(3);
        sqlGenerator.GetUpdate(entity);

        Assert.InRange(entity.UpdatedAt.Value, before, DateTime.UtcNow.AddHours(3));
    }

    [Fact]
    public static void BulkInsertSetsEveryEntity()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var users = new List<User> { new() { Name = "John" }, new() { Name = "Jane" } };

        sqlGenerator.GetBulkInsert(users);

        Assert.All(users, user => Assert.NotNull(user.UpdatedAt));
    }

    [Fact]
    public static void BulkUpdateSetsEveryEntity()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var users = new List<User> { new() { Id = 10, Name = "John" }, new() { Id = 11, Name = "Jane" } };

        sqlGenerator.GetBulkUpdate(users);

        Assert.All(users, user => Assert.NotNull(user.UpdatedAt));
    }

    [Fact]
    public static void EntityWithoutUpdatedAtGetsNoColumn()
    {
        var sqlGenerator = new SqlGenerator<Phone>(_sqlConnector);
        var sqlQuery = sqlGenerator.GetUpdateColumns(new Phone { Id = 10, PNumber = "111" }, x => x.PNumber);

        Assert.Equal("UPDATE DAB.Phones SET DAB.Phones.PNumber = @PhonePNumber WHERE DAB.Phones.Id = @PhoneId", sqlQuery.GetSql());
    }

    [Fact]
    public static void IgnoreUpdateKeepsColumnOutOfUpdate()
    {
        var sqlGenerator = new SqlGenerator<UpdatedAtIgnored>(_sqlConnector);
        var entity = new UpdatedAtIgnored { Id = 10, Name = "John" };

        Assert.Equal("UPDATE UpdatedAtIgnored SET UpdatedAtIgnored.Name = @UpdatedAtIgnoredName WHERE UpdatedAtIgnored.Id = @UpdatedAtIgnoredId",
            sqlGenerator.GetUpdate(entity).GetSql());

        Assert.Equal("UPDATE UpdatedAtIgnored SET UpdatedAtIgnored.Name = @UpdatedAtIgnoredName WHERE UpdatedAtIgnored.Id = @UpdatedAtIgnoredId",
            sqlGenerator.GetUpdateColumns(entity, x => x.Name).GetSql());

        Assert.Equal("UPDATE UpdatedAtIgnored SET UpdatedAtIgnored.Name = @UpdatedAtIgnoredName WHERE UpdatedAtIgnored.Id = @Id_p0",
            sqlGenerator.GetUpdate(x => x.Id == 10, new { Name = "John" }).GetSql());
    }

    [Fact]
    public static void ListedColumnIsNotDuplicated()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var user = new User { Id = 10, Name = "John" };

        var before = DateTime.UtcNow;
        var sqlQuery = sqlGenerator.GetUpdateColumns(user, x => x.UpdatedAt);

        Assert.Equal("UPDATE Users SET Users.UpdatedAt = @UserUpdatedAt WHERE Users.Id = @UserId", sqlQuery.GetSql());
        Assert.InRange(user.UpdatedAt.Value, before, DateTime.UtcNow);
    }

    [Fact]
    public static void ExplicitValueIsKept()
    {
        var sqlGenerator = new SqlGenerator<User>(_sqlConnector);
        var updatedAt = new DateTime(2020, 1, 1);

        var sqlQuery = sqlGenerator.GetUpdate(x => x.Id == 10, new { Name = "John", UpdatedAt = updatedAt });

        Assert.Equal("UPDATE Users SET Users.Name = @UserName, Users.UpdatedAt = @UserUpdatedAt WHERE Users.Id = @Id_p0", sqlQuery.GetSql());

        var parameters = sqlQuery.Param as IDictionary<string, object>;
        Assert.Equal(updatedAt, parameters["UserUpdatedAt"]);
    }
}
