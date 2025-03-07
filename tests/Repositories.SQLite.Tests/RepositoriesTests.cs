using System;
using System.Threading;
using System.Threading.Tasks;
using Repositories.Base;
using TestClasses;
using Xunit;

namespace Repositories.SQLite.Tests;

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
    public void UpdateBinaryDataToNull()
    {
        var guid = Guid.NewGuid();

        var car = new Car
        {
            Data = guid.ToByteArray(),
            Name = "Car",
            Status = StatusCar.Active
        };

        var insert = Db.Cars.Insert(car);
        Assert.True(insert);
        var carFromDb = Db.Cars.Find(x => x.Id == car.Id);
        Assert.NotNull(carFromDb!.Data);

        car.Data = null;
        var update = Db.Cars.Update(car);
        Assert.True(update);
        carFromDb = Db.Cars.Find(x => x.Id == car.Id);
        Assert.Null(carFromDb!.Data);
    }

    [Fact]
    public async Task UpdateBinaryDataToNullAsync()
    {
        var guid = Guid.NewGuid();

        var car = new Car
        {
            Data = guid.ToByteArray(),
            Name = "Car",
            Status = StatusCar.Active
        };

        var insert = await Db.Cars.InsertAsync(car, TestContext.Current.CancellationToken);
        Assert.True(insert);
        var carFromDb = await Db.Cars.FindAsync(x => x.Id == car.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(carFromDb!.Data);

        car.Data = null;
        var update = await Db.Cars.UpdateAsync(car);
        Assert.True(update);
        carFromDb = await Db.Cars.FindAsync(x => x.Id == car.Id, TestContext.Current.CancellationToken);
        Assert.Null(carFromDb!.Data);
    }
}
