using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.Config;
using TestClasses;
using Xunit;

// ReSharper disable PossibleNullReferenceException

namespace Repositories.Base;

public abstract class BaseRepositoriesTests
{
    protected readonly TestDbContext Db;

    protected BaseRepositoriesTests(TestDbContext db)
    {
        Db = db;
        MicroOrmConfig.AllowKeyAsIdentity = false;
    }

    [Fact]
    public async Task ChangeDateInsertAndFind()
    {
        const int diff = 12;
        var dateTime = DateTime.Now.AddDays(-diff);
        var user = new User { Name = "Sergey Phoenix", UpdatedAt = dateTime };
        await Db.Users.InsertAsync(user);
        var userFromDb = await Db.Users.FindAsync(q => q.Id == user.Id);

        Assert.Equal(1, userFromDb.UpdatedAt.GetValueOrDefault().CompareTo(dateTime));
    }

    [Fact]
    public void Count()
    {
        var count = Db.Users.Count();
        var countHandQuery = Db.Connection
            .ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Users.Deleted != " + Db.Users.SqlGenerator.LogicalDeleteValue);
        Assert.Equal(countHandQuery, count);
    }

    [Fact]
    public void CountWithCondition()
    {
        var count = Db.Users.Count(x => x.Id == 2);

        Assert.Equal(1, count);
    }

    [Fact]
    public void CountWithDistinct()
    {
        var count = Db.Phones.Count(phone => phone.Code);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CountWithDistinctAsync()
    {
        var count = await Db.Phones.CountAsync(phone => phone.Code);

        Assert.Equal(1, count);
    }

    [Fact]
    public void CountWithDistinctAndWhere()
    {
        var count = Db.Users.Count(x => x.PhoneId == 1, user => user.PhoneId);

        Assert.Equal(1, count);
    }

    [Fact]
    public void Find()
    {
        var user = Db.Users.Find(x => x.Id == 2);
        Assert.False(user.Deleted);
        Assert.Equal("TestName1", user.Name);
    }

    [Fact]
    public void FindById()
    {
        var user = Db.Users.FindById(2);
        Assert.False(user.Deleted);
        Assert.Equal("TestName1", user.Name);
    }

    [Fact]
    public void FindById_MultiKeys()
    {
        Assert.NotNull(Db.Reports.FindById(new[] { 1, 2 }));
    }

    [Fact]
    public async void FindByIdAsync()
    {
        var user = await Db.Users.FindByIdAsync(2);
        Assert.False(user.Deleted);
        Assert.Equal("TestName1", user.Name);
    }

    [Fact]
    public async void FindByIdAsync_WithJoins_NotNull()
    {
        var user = await Db.Users.FindByIdAsync<Car, Phone, Address>(1, x => x.Cars, x => x.Phone, x => x.Addresses);
        Assert.False(user.Deleted);
        Assert.Equal("TestName0", user.Name);

        Assert.NotNull(user.Phone);
        Assert.NotNull(user.Cars);
        Assert.NotNull(user.Addresses);
    }

    [Fact]
    public async void FindByIdAsync_WithJoins_CheckCount()
    {
        var address = await Db.Address.FindByIdAsync<User>(1, x => x.Users);
        Assert.Equal("Street0", address.Street);
        Assert.NotNull(address.Users);
        Assert.True(address.Users.Count == 10);
    }

    [Fact]
    public async void FindAllAsync_WithPredicate_CheckCount()
    {
        var addresses = (await Db.Address.FindAllAsync<User>(q => q.Id == 1, x => x.Users)).ToArray();
        Assert.Single(addresses);
        var address = addresses.Single();
        Assert.Equal("Street0", address.Street);
        Assert.NotNull(address.Users);
        Assert.True(address.Users.Count == 10);
    }

    [Fact]
    public async void FindAllAsync_NullPredicate_CheckCount()
    {
        var addresses = (await Db.Address.FindAllAsync<User>(null, x => x.Users)).ToArray();
        var address = addresses.First();
        Assert.Equal("Street0", address.Street);
        Assert.NotNull(address.Users);
        Assert.True(address.Users.Count == 10);
    }

    [Fact]
    protected void FindJoin_CollectionnRecord()
    {
        var user = Db.Users.Find<Car>(q => q.Id == 1, q => q.Cars);
        Assert.False(user.Deleted);
        Assert.Equal("TestName0", user.Name);

        Assert.True(user.Cars.Count == 2);
    }

    [Fact]
    public async void FindJoinAsync_CollectionnRecord()
    {
        var user = await Db.Users.FindAsync<Car>(q => q.Id == 1, q => q.Cars);
        Assert.False(user.Deleted);
        Assert.Equal("TestName0", user.Name);

        Assert.True(user.Cars.Count == 2);
    }


    [Fact]
    public async Task FindThroughtNavigationProperty()
    {
        var user = await Db.Users.FindAsync<Phone>(x => x.Phone.PNumber == "123", x => x.Phone);
        Assert.Equal("TestName0", user.Name);

        var user1 = await Db.Users.FindAsync<Phone>(x => x.Phone.PNumber == "2223", x => x.Phone);
        Assert.Null(user1);
    }

    [Fact]
    public async Task FindAllByContainsMultipleList()
    {
        var keyList = new List<int> { 2, 3, 4 };
        var users = (await Db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
        var usersArray = users.ToArray();
        Assert.Equal(3, usersArray.Length);
        Assert.Equal("TestName1", usersArray[0].Name);
        Assert.Equal("TestName2", usersArray[1].Name);
        Assert.Equal("TestName3", usersArray[2].Name);
    }

    [Fact]
    public async Task FindAllByContainsEmptyList()
    {
        var keyList = new List<int>();
        var users = (await Db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
        Assert.Empty(users);
    }

    [Fact]
    public async Task FindAllAsync()
    {
        var users = (await Db.Users.FindAllAsync(x => x.Name == "TestName0")).ToArray();
        Assert.Equal(2, users.Length);

        var user1 = users.FirstOrDefault(x => x.Id == 1);
        Assert.NotNull(user1);

        var user2 = users.FirstOrDefault(x => x.Id == 2);
        Assert.Null(user2);

        var user11 = users.FirstOrDefault(x => x.Id == 11);
        Assert.NotNull(user11);
    }

    [Fact]
    public async Task FindPhoneAsyncDifferentBoolQuery()
    {
        var phone = await Db.Phones.FindAsync(x => x.Id == 1);
        Assert.NotNull(phone);

        var phone1 = await Db.Phones.FindAsync(x => x.IsActive);
        Assert.NotNull(phone1);

        var phone2 = await Db.Phones.FindAsync(x => x.PNumber == "123" && !x.IsActive);
        Assert.Null(phone2);

        var phone3 = await Db.Phones.FindAsync(x => x.Id == 1 && x.IsActive);
        Assert.NotNull(phone3);

        var phone4 = await Db.Phones.FindAsync(x => x.PNumber == "333" && !x.IsActive);
        Assert.NotNull(phone4);
    }

    [Fact]
    public void FindAllJoin2Table()
    {
        var user = Db.Users.FindAll<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses).First();
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
        Assert.Equal("Street0", user.Addresses.Street);
    }

    [Fact]
    public async Task FindAllJoin2TableAsync()
    {
        var user = (await Db.Users.FindAllAsync<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses)).First();
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
        Assert.Equal("Street0", user.Addresses.Street);
    }

    [Fact]
    public void FindAllJoin3Table()
    {
        var user = Db.Users.FindAll<Car, Address, Phone>(x => x.Id == 1, q => q.Cars, q => q.Addresses, q => q.Phone).First();
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
        Assert.Equal("Street0", user.Addresses.Street);
        Assert.Equal("123", user.Phone.PNumber);
    }

    [Fact]
    public async Task FindAllJoin3TableAsync()
    {
        var user = (await Db.Users.FindAllAsync<Car, Address, Phone>(x => x.Id == 1, q => q.Cars, q => q.Addresses, q => q.Phone)).First();
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
        Assert.Equal("Street0", user.Addresses.Street);
        Assert.Equal("123", user.Phone.PNumber);
    }

    [Fact]
    public void FindAllJoinSameTableTwice()
    {
        var user = Db.Users.FindAll<Phone, Car, Phone>(x => x.Id == 1, q => q.OfficePhone, q => q.Cars, q => q.Phone).First();
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
        Assert.Equal("333", user.OfficePhone.PNumber);
        Assert.Equal("123", user.Phone.PNumber);
    }

    [Fact]
    public async void FindAllJoinSameTableTwiceAsync()
    {
        var user = (await Db.Users.FindAllAsync<Phone, Car, Phone>(x => x.Id == 1, q => q.OfficePhone, q => q.Cars, q => q.Phone)).First();
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
        Assert.Equal("333", user.OfficePhone.PNumber);
        Assert.Equal("123", user.Phone.PNumber);
    }

    [Fact]
    public async Task FindAsync()
    {
        const int id = 4;
        const string name = "TestName3";
        {
            var user = await Db.Users.FindAsync(x => x.Id == id);
            Assert.False(user.Deleted);
            Assert.Equal(name, user.Name);
        }
        {
            var user = await Db.Users.FindAsync(x => x.Name == name);
            Assert.Equal(id, user.Id);

            Assert.Null(user.Cars);
        }
    }

    [Fact]
    public void FindJoin_User()
    {
        var user = Db.Users.Find<Car>(x => x.Id == 1, q => q.Cars);
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
    }

    [Fact]
    public void FindJoin_Car()
    {
        var car = Db.Cars.Find<User>(x => x.Id == 1, q => q.User);
        Assert.NotNull(car);
        Assert.NotNull(car.User);
        Assert.Equal("TestName0", car.User.Name);
    }

    [Fact]
    public async Task FindJoinAsync_User()
    {
        var user = await Db.Users.FindAsync<Car>(x => x.Id == 1, q => q.Cars);
        Assert.True(user.Cars.Count == 2);
        Assert.Equal("TestCar0", user.Cars.First().Name);
    }

    [Fact]
    public async Task FindJoinAsync_Car()
    {
        var car = await Db.Cars.FindAsync<User>(x => x.Id == 1, q => q.User);
        Assert.NotNull(car);
        Assert.NotNull(car.User);
        Assert.Equal("TestName0", car.User.Name);
    }

    [Fact]
    public void InsertAndUpdate()
    {
        var user = new User
        {
            Name = "Sergey"
        };

        var insert = Db.Users.Insert(user);
        Assert.True(insert);

        var userFromDb = Db.Users.Find(q => q.Id == user.Id);
        Assert.Equal(user.Name, userFromDb.Name);
        user.Name = "Sergey1";

        var update = Db.Users.Update(user);
        Assert.True(update);
        userFromDb = Db.Users.Find(q => q.Id == user.Id);
        Assert.Equal("Sergey1", userFromDb.Name);
    }

    [Fact]
    public async Task InsertAndUpdateAsync()
    {
        var user = new User
        {
            Name = "Sergey"
        };

        var insert = await Db.Users.InsertAsync(user);
        Assert.True(insert);

        var userFromDb = await Db.Users.FindAsync(q => q.Id == user.Id);
        Assert.Equal(user.Name, userFromDb.Name);
        user.Name = "Sergey1";

        var update = await Db.Users.UpdateAsync(user);
        Assert.True(update);
        userFromDb = await Db.Users.FindAsync(q => q.Id == user.Id);
        Assert.Equal("Sergey1", userFromDb.Name);
    }

    [Fact]
    public void InsertBinaryData()
    {
        var guid = Guid.NewGuid();

        var car = new Car
        {
            Data = guid.ToByteArray(),
            Name = "NewNew Data",
            Status = StatusCar.Active
        };

        var insert = Db.Cars.Insert(car);
        Assert.True(insert);
        var carFromDb = Db.Cars.Find(x => x.Id == car.Id);
        var guid2 = new Guid(carFromDb.Data);
        Assert.Equal(guid, guid2);
    }

    [Fact]
    public async Task InsertBinaryDataAsync()
    {
        var guid = Guid.NewGuid();

        var car = new Car
        {
            Data = guid.ToByteArray(),
            Name = "NewNew Data",
            Status = StatusCar.Active
        };

        var insert = await Db.Cars.InsertAsync(car);
        Assert.True(insert);
        var carFromDb = await Db.Cars.FindAsync(x => x.Id == car.Id);
        var guid2 = new Guid(carFromDb.Data);
        Assert.Equal(guid, guid2);
    }


    [Fact]
    public async Task LogicalDeletedBoolAsync()
    {
        const int id = 10;

        var user = await Db.Users.FindAsync(x => x.Id == id);
        Assert.False(user.Deleted);

        var deleted = await Db.Users.DeleteAsync(user);
        Assert.True(deleted);

        var deletedUser = await Db.Users.FindAsync(x => x.Id == id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task LogicalDeletedEnumAsync()
    {
        var newCar = new Car
        {
            Data = Guid.NewGuid().ToByteArray(),
            Name = "Deleted Car",
            Status = StatusCar.Active
        };

        var insert = await Db.Cars.InsertAsync(newCar);

        var car = await Db.Cars.FindAsync(x => x.Id == newCar.Id);
        Assert.False(car.Status == StatusCar.Deleted);

        var deleted = await Db.Cars.DeleteAsync(car);
        Assert.True(deleted);

        var deletedCar = await Db.Cars.FindAsync(x => x.Id == newCar.Id);
        Assert.Null(deletedCar);
    }

    [Fact]
    public async Task TransactionTest()
    {
        var user = new User
        {
            Name = "Sergey_Transaction"
        };
        using (var trans = Db.BeginTransaction())
        {
            await Db.Users.InsertAsync(user, trans);
            trans.Rollback();
        }

        var userFromDb = await Db.Users.FindAsync(x => x.Name == "Sergey_Transaction");
        Assert.Null(userFromDb);

        using (var trans = Db.BeginTransaction())
        {
            await Db.Users.InsertAsync(user, trans);
            trans.Commit();
        }

        userFromDb = await Db.Users.FindAsync(x => x.Name == "Sergey_Transaction");
        Assert.NotNull(userFromDb);
    }

    [Fact]
    public void FindAllJoinWithoutKeyTable()
    {
        try
        {
            var address = Db.Address.FindAll<City>(x => x.Id == 1, q => q.City).First();
        }
        catch (NotSupportedException e)
        {
            Assert.Equal("Join doesn't support without [Key] attribute", e.Message);
        }
    }

    [Fact]
    public async Task FindAsyncJoinCompositeKey()
    {
        await Db.Reports.InsertAsync(new Report
        {
            Id = 20,
            AnotherId = 20000,
            UserId = 1
        });

        var reportOne = await Db.Reports.FindAsync<User>(x => x.Id == 20, q => q.User);
        Assert.Equal("TestName0", reportOne.User.Name);

        await Db.Reports.InsertAsync(new Report
        {
            Id = 30,
            AnotherId = 20000,
            UserId = 1
        });

        var reportAll = (await Db.Reports.FindAllAsync<User>(x => x.AnotherId == 20000, q => q.User)).ToArray();
        Assert.Equal(2, reportAll.Length);

        foreach (var report in reportAll)
        {
            Assert.Equal("TestName0", report.User.Name);
        }
    }

    [Fact]
    public async Task BulkInsertAsync()
    {
        var adresses = new List<Address>
        {
            new Address { Street = "aaa0", CityId = "10" },
            new Address { Street = "aaa1", CityId = "11" }
        };

        int inserted = await Db.Address.BulkInsertAsync(adresses);
        Assert.Equal(2, inserted);

        var adresses0 = await Db.Address.FindAsync(x => x.CityId == "10");
        var adresses1 = await Db.Address.FindAsync(x => x.CityId == "11");

        Assert.Equal("aaa0", adresses0.Street);
        Assert.Equal("aaa1", adresses1.Street);
    }

    [Fact]
    public void BulkInsert()
    {
        var adresses = new List<Address>
        {
            new Address { Street = "aaa0", CityId = "10" },
            new Address { Street = "aaa1", CityId = "11" }
        };

        int inserted = Db.Address.BulkInsert(adresses);
        Assert.Equal(2, inserted);

        var adresses0 = Db.Address.Find(x => x.CityId == "10");
        var adresses1 = Db.Address.Find(x => x.CityId == "11");

        Assert.Equal("aaa0", adresses0.Street);
        Assert.Equal("aaa1", adresses1.Street);
    }

    [Fact]
    public void Delete()
    {
        var adresses = new List<Address>
        {
            new Address { Street = "aaa10", CityId = "110" },
            new Address { Street = "aaa10", CityId = "111" },
            new Address { Street = "aaa10", CityId = "112" }
        };

        int inserted = Db.Address.BulkInsert(adresses);
        Assert.Equal(3, inserted);

        var adresses0 = Db.Address.Find(x => x.CityId == "110");
        var adresses1 = Db.Address.Find(x => x.CityId == "111");
        var objectsCount = Db.Address.FindAll(x => x.Street == "aaa10").Count();

        Assert.Equal(3, objectsCount);

        Assert.Equal("aaa10", adresses0.Street);
        Assert.Equal("aaa10", adresses1.Street);

        Db.Address.Delete(x => x.Street == "aaa10" && x.CityId != "112");

        objectsCount = Db.Address.FindAll(x => x.Street == "aaa10").Count();

        Assert.Equal(1, objectsCount);
    }

    [Fact]
    public void Delete_Timeout()
    {
        var adresses = new List<Address>
        {
            new Address { Street = "xaaa10", CityId = "x110" },
            new Address { Street = "xaaa10", CityId = "x111" },
            new Address { Street = "xaaa10", CityId = "x112" }
        };

        int inserted = Db.Address.BulkInsert(adresses);
        Assert.Equal(3, inserted);

        var adresses0 = Db.Address.Find(x => x.CityId == "x110");
        var adresses1 = Db.Address.Find(x => x.CityId == "x111");
        var objectsCount = Db.Address.FindAll(x => x.Street == "xaaa10").Count();

        Assert.Equal(3, objectsCount);

        Assert.Equal("xaaa10", adresses0.Street);
        Assert.Equal("xaaa10", adresses1.Street);

        Db.Address.Delete(x => x.Street == "xaaa10" && x.CityId != "x112", timeout: TimeSpan.FromSeconds(5));

        objectsCount = Db.Address.FindAll(x => x.Street == "xaaa10").Count();

        Assert.Equal(1, objectsCount);
    }

    [Fact]
    public void BulkUpdate()
    {
        var user1 = new User
        {
            Name = "Bulk1",
            PhoneId = 1,
            OfficePhoneId = 2
        };
        var user2 = new User
        {
            Name = "Bulk2",
            PhoneId = 1,
            OfficePhoneId = 2
        };

        Db.Users.Insert(user1);
        Db.Users.Insert(user2);

        var insertedUser1 = Db.Users.FindById(user1.Id);
        var insertedUser2 = Db.Users.FindById(user2.Id);
        Assert.Equal("Bulk1", insertedUser1.Name);
        Assert.Equal("Bulk2", insertedUser2.Name);

        insertedUser1.Name = "Bulk11";
        insertedUser2.Name = "Bulk22";

        bool result = Db.Users.BulkUpdate(new List<User> { insertedUser1, insertedUser2 });

        Assert.True(result);

        var newUser1 = Db.Users.FindById(user1.Id);
        var newUser2 = Db.Users.FindById(user2.Id);

        Assert.Equal("Bulk11", newUser1.Name);
        Assert.Equal("Bulk22", newUser2.Name);
    }

    [Fact]
    public async Task BulkUpdateAsync()
    {
        var user1 = new User
        {
            Name = "Bulk1",
            PhoneId = 1,
            OfficePhoneId = 2
        };
        var user2 = new User
        {
            Name = "Bulk2",
            PhoneId = 1,
            OfficePhoneId = 2
        };

        await Db.Users.InsertAsync(user1);
        await Db.Users.InsertAsync(user2);

        var insertedUser1 = await Db.Users.FindByIdAsync(user1.Id);
        var insertedUser2 = await Db.Users.FindByIdAsync(user2.Id);
        Assert.Equal("Bulk1", insertedUser1.Name);
        Assert.Equal("Bulk2", insertedUser2.Name);

        insertedUser1.Name = "Bulk11";
        insertedUser2.Name = "Bulk22";

        bool result = await Db.Users.BulkUpdateAsync(new List<User> { insertedUser1, insertedUser2 });

        Assert.True(result);

        var newUser1 = await Db.Users.FindByIdAsync(user1.Id);
        var newUser2 = await Db.Users.FindByIdAsync(user2.Id);

        Assert.Equal("Bulk11", newUser1.Name);
        Assert.Equal("Bulk22", newUser2.Name);
    }

    [Fact]
    public async Task LogicalDeleteWithUpdatedAt()
    {
        const string name = "testLogicalDeleted1";
        var user = new User()
        {
            Name = name
        };

        await Db.Users.InsertAsync(user);
        user = await Db.Users.FindAsync(q => q.Name == name);

        //var updatedAt = user.UpdatedAt;

        await Db.Users.DeleteAsync(user);
        user = await Db.Users.FindAsync(q => q.Name == name);

        Assert.Null(user);
    }

    [Fact]
    public async Task LogicalDeleteWithUpdatedAtWithPredicate()
    {
        const string name = "testLogicalDeleted2";
        var user = new User()
        {
            Name = name
        };

        await Db.Users.InsertAsync(user);
        user = await Db.Users.FindAsync(q => q.Name == name);

        // var updatedAt = user.UpdatedAt;

        await Db.Users.DeleteAsync(q => q.Id == user.Id);
        user = await Db.Users.FindAsync(q => q.Name == name);

        Assert.Null(user);
    }

    [Fact]
    public async Task FindAllByContainsArrayMultipleList()
    {
        var keyList = new int[] { 2, 3, 4 };
        var users = (await Db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
        var usersArray = users.ToArray();
        Assert.Equal(3, usersArray.Length);
        Assert.Equal("TestName1", usersArray[0].Name);
        Assert.Equal("TestName2", usersArray[1].Name);
        Assert.Equal("TestName3", usersArray[2].Name);
    }

    [Fact]
    public async Task FindAllByLikeName()
    {
        var users1 = (await Db.Users.FindAllAsync(x => x.Name.EndsWith("Name1"))).ToArray();
        Assert.Equal("TestName1", users1.First().Name);

        var users2 = (await Db.Users.FindAllAsync(x => x.Name.Contains("Name"))).ToArray();
        Assert.True(users2.Length > 0);

        var users3 = (await Db.Users.FindAllAsync(x => x.Name.StartsWith("Test"))).ToArray();
        Assert.True(users3.Length > 0);

        var users4 = (await Db.Users.FindAllAsync(x => !x.Name.StartsWith("est"))).ToArray();
        Assert.True(users4.Length > 0);

        var users5 = (await Db.Users.FindAllAsync(x => x.Name.StartsWith("est"))).ToArray();
        Assert.True(users5.Length <= 0);
    }

    [Fact]
    public async Task CancellationTokenSource_Cancel()
    {
        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Db.Address.FindAllAsync(cts.Token));
    }
}
