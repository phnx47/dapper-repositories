using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DbContexts;
using Xunit;
using Xunit.Abstractions;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public abstract class RepositoriesTests
    {
        private readonly IDbContext _db;
        private readonly ITestOutputHelper _testOutputHelper;

        protected RepositoriesTests(IDbContext db, ITestOutputHelper testOutputHelper)
        {
            _db = db;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ChangeDateInsertAndFind()
        {
            const int diff = 12;
            var dateTime = DateTime.Now.AddDays(-diff);
            var user = new User {Name = "Sergey Phoenix", UpdatedAt = dateTime};
            await _db.Users.InsertAsync(user);
            var userFromDb = await _db.Users.FindAsync(q => q.Id == user.Id);

            Assert.Equal(1, userFromDb.UpdatedAt.Value.CompareTo(dateTime));
        }

        [Fact]
        public void Count()
        {
            var count = _db.Users.Count();
            var countHandQuery =
                _db.Connection
                    .ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Users.Deleted != 1");
            Assert.Equal(countHandQuery, count);
        }

        [Fact]
        public void CountWithCondition()
        {
            var count = _db.Users.Count(x => x.Id == 2);

            Assert.Equal(1, count);
        }

        [Fact]
        public void CountWithDistinct()
        {
            var count = _db.Phones.Count(phone => phone.Code);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CountWithDistinctAsync()
        {
            var count = await _db.Phones.CountAsync(phone => phone.Code);

            Assert.Equal(1, count);
        }

        [Fact]
        public void CountWithDistinctAndWhere()
        {
            var count = _db.Users.Count(x => x.PhoneId == 1, user => user.PhoneId);

            Assert.Equal(1, count);
        }

        [Fact]
        public void Find()
        {
            var user = _db.Users.Find(x => x.Id == 2);
            Assert.False(user.Deleted);
            Assert.Equal("TestName1", user.Name);
        }

        [Fact]
        public void FindById()
        {
            var user = _db.Users.FindById(2);
            Assert.False(user.Deleted);
            Assert.Equal("TestName1", user.Name);
        }

        [Fact]
        public async void FindByIdAsync()
        {
            var user = await _db.Users.FindByIdAsync(2);
            Assert.False(user.Deleted);
            Assert.Equal("TestName1", user.Name);
        }

        [Fact]
        public async void FindByIdWithJoinAsync_NotNullJoins()
        {
            var user = await _db.Users.FindByIdAsync<Car, Phone, Address>(1, x => x.Cars, x => x.Phone, x => x.Addresses);
            Assert.False(user.Deleted);
            Assert.Equal("TestName0", user.Name);

            Assert.NotNull(user.Phone);
            Assert.NotNull(user.Cars);
            Assert.NotNull(user.Addresses);
        }

        [Fact]
        public void FindJoin_CollectionneRecord()
        {
            var user = _db.Users.Find<Car>(q => q.Id == 1, q => q.Cars);
            Assert.False(user.Deleted);
            Assert.Equal("TestName0", user.Name);

            Assert.True(user.Cars.Count == 1);
        }

        [Fact]
        public async void FindJoinAsync_CollectionneRecord()
        {
            var user = await _db.Users.FindAsync<Car>(q => q.Id == 1, q => q.Cars);
            Assert.False(user.Deleted);
            Assert.Equal("TestName0", user.Name);

            Assert.True(user.Cars.Count == 1);
        }

        [Fact]
        public async Task FindThroughtNavigationProperty()
        {
            var user = await _db.Users.FindAsync<Phone>(x => x.Phone.Number == "123", x => x.Phone);
            Assert.Equal("TestName0", user.Name);

            var user1 = await _db.Users.FindAsync<Phone>(x => x.Phone.Number == "2223", x => x.Phone);
            Assert.Null(user1);
        }

        [Fact]
        public async Task FindAllByContainsMultipleList()
        {
            List<int> keyList = new List<int> {2, 3, 4};
            var users = (await _db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
            var usersArray = users.ToArray();
            Assert.Equal(3, usersArray.Length);
            Assert.Equal("TestName1", usersArray[0].Name);
            Assert.Equal("TestName2", usersArray[1].Name);
            Assert.Equal("TestName3", usersArray[2].Name);
        }

        [Fact]
        public async Task FindAllByContainsEmptyList()
        {
            List<int> keyList = new List<int>();
            var users = (await _db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
            Assert.Empty(users);
        }

        [Fact]
        public async Task FindAllAsync()
        {
            var users = (await _db.Users.FindAllAsync(x => x.Name == "TestName0")).ToArray();
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
            var phone = await _db.Phones.FindAsync(x => x.Id == 1);
            Assert.NotNull(phone);

            var phone1 = await _db.Phones.FindAsync(x => x.IsActive);
            Assert.NotNull(phone1);

            var phone2 = await _db.Phones.FindAsync(x => x.Number == "123" && !x.IsActive);
            Assert.Null(phone2);

            var phone3 = await _db.Phones.FindAsync(x => x.Id == 1 && x.IsActive);
            Assert.NotNull(phone3);

            var phone4 = await _db.Phones.FindAsync(x => x.Number == "333" && !x.IsActive);
            Assert.NotNull(phone4);
        }

        [Fact]
        public void FindAllJoin2Table()
        {
            var user = _db.Users.FindAll<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses).First();
            Assert.True(user.Cars.Count == 2);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
        }

        [Fact]
        public async Task FindAllJoin2TableAsync()
        {
            var user = (await _db.Users.FindAllAsync<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses)).First();
            Assert.True(user.Cars.Count == 2);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
        }

        [Fact]
        public void FindAllJoin3Table()
        {
            var user = _db.Users.FindAll<Car, Address, Phone>(x => x.Id == 1, q => q.Cars, q => q.Addresses, q => q.Phone).First();
            Assert.True(user.Cars.Count == 2);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
            Assert.Equal("123", user.Phone.Number);
        }

        [Fact]
        public async Task FindAllJoin3TableAsync()
        {
            var user = (await _db.Users.FindAllAsync<Car, Address, Phone>(x => x.Id == 1, q => q.Cars, q => q.Addresses, q => q.Phone)).First();
            Assert.True(user.Cars.Count == 2);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
            Assert.Equal("123", user.Phone.Number);
        }

        [Fact]
        public void FindAllJoinSameTableTwice()
        {
            var user = _db.Users.FindAll<Phone, Car, Phone>(x => x.Id == 1, q => q.OfficePhone, q => q.Cars, q => q.Phone).First();
            Assert.True(user.Cars.Count == 2);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("333", user.OfficePhone.Number);
            Assert.Equal("123", user.Phone.Number);
        }

        [Fact]
        public async void FindAllJoinSameTableTwiceAsync()
        {
            var user = (await _db.Users.FindAllAsync<Phone, Car, Phone>(x => x.Id == 1, q => q.OfficePhone, q => q.Cars, q => q.Phone)).First();
            Assert.True(user.Cars.Count == 2);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("333", user.OfficePhone.Number);
            Assert.Equal("123", user.Phone.Number);
        }

        [Fact]
        public async Task FindAsync()
        {
            const int id = 4;
            const string name = "TestName3";
            {
                var user = await _db.Users.FindAsync(x => x.Id == id);
                Assert.False(user.Deleted);
                Assert.Equal(name, user.Name);
            }
            {
                var user = await _db.Users.FindAsync(x => x.Name == name);
                Assert.Equal(id, user.Id);

                Assert.Null(user.Cars);
            }
        }

        [Fact]
        public void FindJoin_User()
        {
            var user = _db.Users.Find<Car>(x => x.Id == 1, q => q.Cars);
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);
        }

        [Fact]
        public void FindJoin_Car()
        {
            var car = _db.Cars.Find<User>(x => x.Id == 1, q => q.User);
            Assert.NotNull(car);
            Assert.NotNull(car.User);
            Assert.Equal("TestName0", car.User.Name);
        }

        [Fact]
        public async Task FindJoinAsync_User()
        {
            var user = await _db.Users.FindAsync<Car>(x => x.Id == 1, q => q.Cars);
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);
        }

        [Fact]
        public async Task FindJoinAsync_Car()
        {
            var car = await _db.Cars.FindAsync<User>(x => x.Id == 1, q => q.User);
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

            var insert = _db.Users.Insert(user);
            Assert.True(insert);

            var userFromDb = _db.Users.Find(q => q.Id == user.Id);
            Assert.Equal(user.Name, userFromDb.Name);
            user.Name = "Sergey1";

            var update = _db.Users.Update(user);
            Assert.True(update);
            userFromDb = _db.Users.Find(q => q.Id == user.Id);
            Assert.Equal("Sergey1", userFromDb.Name);
        }

        [Fact]
        public async Task InsertAndUpdateAsync()
        {
            var user = new User
            {
                Name = "Sergey"
            };

            var insert = await _db.Users.InsertAsync(user);
            Assert.True(insert);

            var userFromDb = await _db.Users.FindAsync(q => q.Id == user.Id);
            Assert.Equal(user.Name, userFromDb.Name);
            user.Name = "Sergey1";

            var update = await _db.Users.UpdateAsync(user);
            Assert.True(update);
            userFromDb = await _db.Users.FindAsync(q => q.Id == user.Id);
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

            var insert = _db.Cars.Insert(car);
            Assert.True(insert);
            var carFromDb = _db.Cars.Find(x => x.Id == car.Id);
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

            var insert = await _db.Cars.InsertAsync(car);
            Assert.True(insert);
            var carFromDb = await _db.Cars.FindAsync(x => x.Id == car.Id);
            var guid2 = new Guid(carFromDb.Data);
            Assert.Equal(guid, guid2);
        }

        [Fact]
        public async Task LogicalDeletedBoolAsync()
        {
            const int id = 10;

            var user = _db.Users.Find(x => x.Id == id);
            Assert.False(user.Deleted);

            var deleted = await _db.Users.DeleteAsync(user);
            Assert.True(deleted);

            var deletedUser = _db.Users.Find(x => x.Id == id);
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

            var insert = await _db.Cars.InsertAsync(newCar);

            var car = _db.Cars.Find(x => x.Id == newCar.Id);
            Assert.False(car.Status == StatusCar.Deleted);

            var deleted = await _db.Cars.DeleteAsync(car);
            Assert.True(deleted);

            var deletedCar = _db.Cars.Find(x => x.Id == newCar.Id);
            Assert.Null(deletedCar);
        }

        [Fact]
        public async Task TransactionTest()
        {
            var user = new User
            {
                Name = "Sergey_Transaction"
            };
            using (var trans = _db.BeginTransaction())
            {
                await _db.Users.InsertAsync(user, trans);
                trans.Rollback();
            }

            var userFromDb = await _db.Users.FindAsync(x => x.Name == "Sergey_Transaction");
            Assert.Null(userFromDb);

            using (var trans = _db.BeginTransaction())
            {
                await _db.Users.InsertAsync(user, trans);
                trans.Commit();
            }

            userFromDb = await _db.Users.FindAsync(x => x.Name == "Sergey_Transaction");
            Assert.NotNull(userFromDb);
        }

        [Fact]
        public void FindAllJoinWithoutKeyTable()
        {
            try
            {
                var address = _db.Address.FindAll<City>(x => x.Id == 1, q => q.City).First();
            }
            catch (NotSupportedException e)
            {
                Assert.Equal("Join doesn't support without [Key] attribute", e.Message);
            }
        }

        [Fact]
        public async Task FindAsyncJoinCompositeKey()
        {
            await _db.Reports.InsertAsync(new Report
            {
                Id = 20,
                AnotherId = 20000,
                UserId = 1
            });

            var reportOne = await _db.Reports.FindAsync<User>(x => x.Id == 20, q => q.User);
            Assert.Equal("TestName0", reportOne.User.Name);

            await _db.Reports.InsertAsync(new Report
            {
                Id = 30,
                AnotherId = 20000,
                UserId = 1
            });

            var reportAll = (await _db.Reports.FindAllAsync<User>(x => x.AnotherId == 20000, q => q.User)).ToArray();
            Assert.Equal(2, reportAll.Length);

            foreach (var report in reportAll)
            {
                Assert.Equal("TestName0", report.User.Name);
            }
        }

        [Fact]
        public async Task BulkInsertAsync()
        {
            List<Address> adresses = new List<Address>
            {
                new Address {Street = "aaa0", CityId = "10"},
                new Address {Street = "aaa1", CityId = "11"}
            };

            int inserted = await _db.Address.BulkInsertAsync(adresses);
            Assert.Equal(2, inserted);

            var adresses0 = await _db.Address.FindAsync(x => x.CityId == "10");
            var adresses1 = await _db.Address.FindAsync(x => x.CityId == "11");

            Assert.Equal("aaa0", adresses0.Street);
            Assert.Equal("aaa1", adresses1.Street);
        }

        [Fact]
        public void BulkInsert()
        {
            List<Address> adresses = new List<Address>
            {
                new Address {Street = "aaa0", CityId = "10"},
                new Address {Street = "aaa1", CityId = "11"}
            };

            int inserted = _db.Address.BulkInsert(adresses);
            Assert.Equal(2, inserted);

            var adresses0 = _db.Address.Find(x => x.CityId == "10");
            var adresses1 = _db.Address.Find(x => x.CityId == "11");

            Assert.Equal("aaa0", adresses0.Street);
            Assert.Equal("aaa1", adresses1.Street);
        }

        [Fact]
        public void Delete()
        {
            List<Address> adresses = new List<Address>
            {
                new Address {Street = "aaa10", CityId = "110"},
                new Address {Street = "aaa10", CityId = "111"},
                new Address {Street = "aaa10", CityId = "112"}
            };

            int inserted = _db.Address.BulkInsert(adresses);
            Assert.Equal(3, inserted);

            var adresses0 = _db.Address.Find(x => x.CityId == "110");
            var adresses1 = _db.Address.Find(x => x.CityId == "111");
            var objectsCount = _db.Address.FindAll(x => x.Street == "aaa10").Count();

            Assert.Equal(3, objectsCount);

            Assert.Equal("aaa10", adresses0.Street);
            Assert.Equal("aaa10", adresses1.Street);

            _db.Address.Delete(x => x.Street == "aaa10" && x.CityId != "112");

            objectsCount = _db.Address.FindAll(x => x.Street == "aaa10").Count();

            Assert.Equal(1, objectsCount);
        }

        [Fact]
        public void BulkUpdate()
        {
            var user1 = new User
            {
                Name = "Bulk1",
                AddressId = 1,
                PhoneId = 1,
                OfficePhoneId = 2
            };
            var user2 = new User
            {
                Name = "Bulk2",
                AddressId = 1,
                PhoneId = 1,
                OfficePhoneId = 2
            };

            _db.Users.Insert(user1);
            _db.Users.Insert(user2);

            var insertedUser1 = _db.Users.FindById(user1.Id);
            var insertedUser2 = _db.Users.FindById(user2.Id);
            Assert.Equal("Bulk1", insertedUser1.Name);
            Assert.Equal("Bulk2", insertedUser2.Name);

            insertedUser1.Name = "Bulk11";
            insertedUser2.Name = "Bulk22";

            bool result = _db.Users.BulkUpdate(new List<User> {insertedUser1, insertedUser2});

            Assert.True(result);

            var newUser1 = _db.Users.FindById(user1.Id);
            var newUser2 = _db.Users.FindById(user2.Id);

            Assert.Equal("Bulk11", newUser1.Name);
            Assert.Equal("Bulk22", newUser2.Name);
        }

        [Fact]
        public async void BulkUpdateAsync()
        {
            var user1 = new User
            {
                Name = "Bulk1",
                AddressId = 1,
                PhoneId = 1,
                OfficePhoneId = 2
            };
            var user2 = new User
            {
                Name = "Bulk2",
                AddressId = 1,
                PhoneId = 1,
                OfficePhoneId = 2
            };

            _db.Users.Insert(user1);
            _db.Users.Insert(user2);

            var insertedUser1 = await _db.Users.FindByIdAsync(user1.Id);
            var insertedUser2 = await _db.Users.FindByIdAsync(user2.Id);
            Assert.Equal("Bulk1", insertedUser1.Name);
            Assert.Equal("Bulk2", insertedUser2.Name);

            insertedUser1.Name = "Bulk11";
            insertedUser2.Name = "Bulk22";

            bool result = await _db.Users.BulkUpdateAsync(new List<User> {insertedUser1, insertedUser2});

            Assert.True(result);

            var newUser1 = await _db.Users.FindByIdAsync(user1.Id);
            var newUser2 = await _db.Users.FindByIdAsync(user2.Id);

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

            await _db.Users.InsertAsync(user);
            user = await _db.Users.FindAsync(q => q.Name == name);

            //var updatedAt = user.UpdatedAt;

            await _db.Users.DeleteAsync(user);
            user = await _db.Users.FindAsync(q => q.Name == name);

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

            await _db.Users.InsertAsync(user);
            user = await _db.Users.FindAsync(q => q.Name == name);

            // var updatedAt = user.UpdatedAt;

            await _db.Users.DeleteAsync(q => q.Id == user.Id);
            user = await _db.Users.FindAsync(q => q.Name == name);

            Assert.Null(user);
        }

        [Fact]
        public void InsertAndUpdate_WithGuid_WithoutKey()
        {
            var identifier = Guid.NewGuid();
            var city = new City {Identifier = identifier, Name = "Moscow"};

            _db.Cities.Insert(city);
            city = _db.Cities.Find(q => q.Identifier == identifier);

            Assert.NotNull(city);
            city.Name = "Moscow1";
            _db.Cities.Update(q => q.Identifier == identifier, city);

            city = _db.Cities.Find(q => q.Identifier == identifier);
            Assert.NotNull(city);
            Assert.Equal("Moscow1", city.Name);
        }

        [Fact]
        public async Task InsertAndUpdate_WithGuid_WithoutKey_Async()
        {
            var identifier = Guid.NewGuid();
            var city = new City {Identifier = identifier, Name = "Moscow"};

            await _db.Cities.InsertAsync(city);
            city = await _db.Cities.FindAsync(q => q.Identifier == identifier);

            Assert.NotNull(city);
            city.Name = "Moscow1";
            await _db.Cities.UpdateAsync(q => q.Identifier == identifier, city);

            city = await _db.Cities.FindAsync(q => q.Identifier == identifier);
            Assert.NotNull(city);
            Assert.Equal("Moscow1", city.Name);
        }

        [Fact]
        public async Task FindAllByContainsArrayMultipleList()
        {
            var keyList = new int[] {2, 3, 4};
            var users = (await _db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
            var usersArray = users.ToArray();
            Assert.Equal(3, usersArray.Length);
            Assert.Equal("TestName1", usersArray[0].Name);
            Assert.Equal("TestName2", usersArray[1].Name);
            Assert.Equal("TestName3", usersArray[2].Name);
        }

        [Fact]
        public async Task FindAllByLikeName()
        {
            var users1 = (await _db.Users.FindAllAsync(x => x.Name.EndsWith("Name1"))).ToArray();
            Assert.Equal("TestName1", users1.First().Name);

            var users2 = (await _db.Users.FindAllAsync(x => x.Name.Contains("Name"))).ToArray();
            Assert.True(users2.Length > 0);

            var users3 = (await _db.Users.FindAllAsync(x => x.Name.StartsWith("Test"))).ToArray();
            Assert.True(users3.Length > 0);

            var users4 = (await _db.Users.FindAllAsync(x => !x.Name.StartsWith("est"))).ToArray();
            Assert.True(users4.Length > 0);

            var users5 = (await _db.Users.FindAllAsync(x => x.Name.StartsWith("est"))).ToArray();
            Assert.True(users5.Length <= 0);
        }
    }
}
