﻿using System;
using System.Threading.Tasks;
using Repositories.Base;
using TestClasses;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Repositories.MySql.Tests;

public abstract class RepositoriesTests<TFixture> : BaseRepositoriesTests, IClassFixture<TFixture>
    where TFixture : DatabaseFixture
{
    protected RepositoriesTests(DatabaseFixture fixture)
        : base(fixture.Db)
    {
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
}
