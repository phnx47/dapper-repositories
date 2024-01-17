using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Repositories.Base;
using TestClasses;
using Xunit;

namespace Repositories.MSSQL.Tests;

public abstract class RepositoriesTests<TFixture> : BaseRepositoriesTests, IClassFixture<TFixture>
    where TFixture : DatabaseFixture
{
    protected RepositoriesTests(TFixture fixture)
        : base(fixture.Db)
    {
    }

    [Fact]
    public async Task CancellationTokenSource_Cancel()
    {
        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Db.Address.FindAllAsync(cts.Token));
    }

    [Fact]
    public void InsertAndUpdate_WithGuid_WithoutKey()
    {
        var identifier = Guid.NewGuid();
        var city = new City { Identifier = identifier, Name = "Moscow" };

        Db.Cities.Insert(city);
        city = Db.Cities.Find(q => q.Identifier == identifier);

        Assert.NotNull(city);
        city.Name = "Moscow1";
        Db.Cities.Update(q => q.Identifier == identifier, city);

        city = Db.Cities.Find(q => q.Identifier == identifier);
        Assert.NotNull(city);
        Assert.Equal("Moscow1", city.Name);
    }

    [Fact]
    public async Task InsertAndUpdate_WithGuid_WithoutKey_Async()
    {
        var identifier = Guid.NewGuid();
        var city = new City { Identifier = identifier, Name = "Moscow" };

        await Db.Cities.InsertAsync(city);
        city = await Db.Cities.FindAsync(q => q.Identifier == identifier);

        Assert.NotNull(city);
        city.Name = "Moscow1";
        await Db.Cities.UpdateAsync(q => q.Identifier == identifier, city);

        city = await Db.Cities.FindAsync(q => q.Identifier == identifier);
        Assert.NotNull(city);
        Assert.Equal("Moscow1", city.Name);
    }

    [Fact]
    public void BulkInsert_LotsOfCities()
    {
        var cities = new List<City>();
        for (int i = 0; i < 17250; i++)
        {
            cities.Add(new City { Identifier = Guid.NewGuid(), Name = $"Bulk City Number {i}" });
        }

        var inserted = Db.Cities.BulkInsert(cities);
        Assert.Equal(inserted, cities.Count);

        var random = new Random();
        var cityName = $"Bulk City Number {random.Next(17250)}";
        var randomcity = Db.Cities.Find(q => q.Name == cityName);
        Assert.NotNull(randomcity);

        DeleteBulkCities();
    }

    [Fact]
    public async Task BulkInsert_LotsOfCities_Async()
    {
        var cities = new List<City>();
        for (int i = 0; i < 17250; i++)
        {
            cities.Add(new City { Identifier = Guid.NewGuid(), Name = $"Bulk City Number {i}" });
        }

        var inserted = await Db.Cities.BulkInsertAsync(cities);
        Assert.Equal(inserted, cities.Count);

        var random = new Random();
        var cityName = $"Bulk City Number {random.Next(17250)}";
        var randomCity = await Db.Cities.FindAsync(q => q.Name == cityName);
        Assert.NotNull(randomCity);

        DeleteBulkCities();
    }

    [Fact]
    public async Task BulkInsertAndUpdate_LotsOfUsers_Async()
    {
        var users = new List<User>();
        for (int i = 0; i < 17250; i++)
        {
            users.Add(new User { Name = $"Bulk User Number {i}", PhoneId = 1, OfficePhoneId = 2 });
        }

        var inserted = await Db.Users.BulkInsertAsync(users);
        Assert.Equal(inserted, users.Count);

        var random = new Random();
        var userName = $"Bulk User Number {random.Next(17250)}";
        var randomUser = await Db.Users.FindAsync(q => q.Name == userName);
        Assert.NotNull(randomUser);

        var insertedUsers = (await Db.Connection.QueryAsync<User>("SELECT * FROM Users WHERE Name LIKE 'Bulk User Number%'")).ToList();
        insertedUsers.ForEach(x => x.Name += " Updated");
        var updated = await Db.Users.BulkUpdateAsync(insertedUsers);
        Assert.True(updated);

        var updatedUsers = (await Db.Connection.QueryAsync<User>("SELECT * FROM Users WHERE Name LIKE 'Bulk User Number%Updated'")).ToList();
        Assert.Equal(17250, updatedUsers.Count);
        DeleteBulkUsers();
    }

    [Fact]
    public void BulkInsertAndUpdate_LotsOfUsers()
    {
        var users = new List<User>();
        for (int i = 0; i < 17250; i++)
        {
            users.Add(new User { Name = $"Bulk User Number {i}", PhoneId = 1, OfficePhoneId = 2 });
        }

        var inserted = Db.Users.BulkInsert(users);
        Assert.Equal(inserted, users.Count);

        var random = new Random();
        var userName = $"Bulk User Number {random.Next(17250)}";
        var randomUser = Db.Users.Find(q => q.Name == userName);
        Assert.NotNull(randomUser);

        var insertedUsers = Db.Connection.Query<User>("SELECT * FROM Users WHERE Name LIKE 'Bulk User Number%'").ToList();
        insertedUsers.ForEach(x => x.Name += " Updated");
        var updated = Db.Users.BulkUpdate(insertedUsers);
        Assert.True(updated);

        var updatedUsers = Db.Connection.Query<User>("SELECT * FROM Users WHERE Name LIKE 'Bulk User Number%Updated'").ToList();
        Assert.Equal(17250, updatedUsers.Count);
        DeleteBulkUsers();
    }

    private void DeleteBulkCities()
    {
        const string sql = "DELETE FROM Cities WHERE Name LIKE 'Bulk City Number%'";
        Db.Connection.Execute(sql);
    }

    private void DeleteBulkUsers()
    {
        const string sql = "DELETE FROM Users WHERE Name LIKE 'Bulk User Number%'";
        Db.Connection.Execute(sql);
    }
}
