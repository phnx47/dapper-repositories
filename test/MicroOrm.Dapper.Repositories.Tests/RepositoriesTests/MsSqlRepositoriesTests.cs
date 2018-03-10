using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class MsSqlRepositoriesTests : IClassFixture<MsSqlDatabaseFixture>
    {
        private readonly MsSqlDatabaseFixture _sqlDatabaseFixture;
        
        public MsSqlRepositoriesTests(MsSqlDatabaseFixture msSqlDatabaseFixture)
        {
            _sqlDatabaseFixture = msSqlDatabaseFixture;
        }

        [Fact]
        public async Task ChangeDateInsertAndFind()
        {
            const int diff = 12;
            var dateTime = DateTime.Now.AddDays(-diff);
            var user = new User { Name = "Sergey Phoenix", UpdatedAt = dateTime };
            await _sqlDatabaseFixture.Db.Users.InsertAsync(user);
            var userFromDb = await _sqlDatabaseFixture.Db.Users.FindAsync(q => q.Id == user.Id);


            Assert.Equal(1, userFromDb.UpdatedAt.Value.CompareTo(dateTime));
        }

        [Fact]
        public void Count()
        {
            var count = _sqlDatabaseFixture.Db.Users.Count();
            var countHandQuery =
                _sqlDatabaseFixture.Db.Connection
                .ExecuteScalar<int>("SELECT COUNT(*) FROM [Users] WHERE [Users].[Deleted] != 1");
            Assert.Equal(countHandQuery, count);
        }

        [Fact]
        public void CountWithCondition()
        {
            var count = _sqlDatabaseFixture.Db.Users.Count(x => x.Id == 2);
   
            Assert.Equal(1, count);
        }


        [Fact]
        public void CountWithDistinct()
        {
            var count = _sqlDatabaseFixture.Db.Phones.Count(phone => phone.Code);

            Assert.Equal(1, count);
        }

        [Fact]
        public void CountWithDistinctAndWhere()
        {
            var count = _sqlDatabaseFixture.Db.Users.Count(x=>x.PhoneId == 1, user => user.PhoneId);

            Assert.Equal(1, count);
        }

        [Fact]
        public void Find()
        {
            var user = _sqlDatabaseFixture.Db.Users.Find(x => x.Id == 2);
            Assert.False(user.Deleted);
            Assert.Equal("TestName1", user.Name);
        }

        [Fact]
        public void FindById()
        {
            var user = _sqlDatabaseFixture.Db.Users.FindById(2);
            Assert.False(user.Deleted);
            Assert.Equal("TestName1", user.Name);
        }

        [Fact]
        public async void FindByIdAsync()
        {
            var user = await _sqlDatabaseFixture.Db.Users.FindByIdAsync(2);
            Assert.False(user.Deleted);
            Assert.Equal("TestName1", user.Name);
        }

        [Fact]
        public async void FindByIdWithJoinAsync()
        {
            var user = await _sqlDatabaseFixture.Db.Users.FindByIdAsync<Car, Phone, Address>(1, x => x.Cars, x => x.Phone, x => x.Addresses);
            Assert.False(user.Deleted);
            Assert.Equal("TestName0", user.Name);

            Assert.NotNull(user.Phone);
            Assert.NotNull(user.Cars);
            Assert.NotNull(user.Addresses);
        }

        [Fact]
        public async Task FindThroughtNavigationProperty()
        {
            var user = await _sqlDatabaseFixture.Db.Users.FindAsync<Phone>(x => x.Phone.Number == "123", x => x.Phone);
            Assert.Equal("TestName0", user.Name);

            var user1 = await _sqlDatabaseFixture.Db.Users.FindAsync<Phone>(x => x.Phone.Number == "2223", x => x.Phone);
            Assert.Null(user1);
        }

        [Fact]
        public async Task FindAllByContainsMultipleList()
        {
            List<int> keyList = new List<int> { 2, 3, 4 };
            var users = (await _sqlDatabaseFixture.Db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
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
            var users = (await _sqlDatabaseFixture.Db.Users.FindAllAsync(x => keyList.Contains(x.Id))).ToArray();
            Assert.Empty(users);
        }

        [Fact]
        public async Task FindAllAsync()
        {
            var users = (await _sqlDatabaseFixture.Db.Users.FindAllAsync(x => x.Name == "TestName0")).ToArray();
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
            var phone = await _sqlDatabaseFixture.Db.Phones.FindAsync(x => x.Id == 1);
            Assert.NotNull(phone);

            var phone1 = await _sqlDatabaseFixture.Db.Phones.FindAsync(x => x.IsActive);
            Assert.NotNull(phone1);

            var phone2 = await _sqlDatabaseFixture.Db.Phones.FindAsync(x => x.Number == "123" && !x.IsActive);
            Assert.Null(phone2);

            var phone3 = await _sqlDatabaseFixture.Db.Phones.FindAsync(x => x.Id == 1 && x.IsActive);
            Assert.NotNull(phone3);

            var phone4 = await _sqlDatabaseFixture.Db.Phones.FindAsync(x => x.Number == "333" && !x.IsActive);
            Assert.NotNull(phone4);
        }


        [Fact]
        public void FindAllJoin2Table()
        {
            var user = _sqlDatabaseFixture.Db.Users.FindAll<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses).First();
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
        }

        [Fact]
        public async Task FindAllJoin2TableAsync()
        {
            var user = (await _sqlDatabaseFixture.Db.Users.FindAllAsync<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses)).First();
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
        }

        [Fact]
        public void FindAllJoin3Table()
        {
            var user = _sqlDatabaseFixture.Db.Users.FindAll<Car, Address, Phone>(x => x.Id == 1, q => q.Cars, q => q.Addresses, q => q.Phone).First();
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
            Assert.Equal("123", user.Phone.Number);
        }

        [Fact]
        public async Task FindAllJoin3TableAsync()
        {
            var user = (await _sqlDatabaseFixture.Db.Users.FindAllAsync<Car, Address, Phone>(x => x.Id == 1, q => q.Cars, q => q.Addresses, q => q.Phone)).First();
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
            Assert.Equal("123", user.Phone.Number);
        }

        [Fact]
        public void FindAllJoinSameTableTwice()
        {
            var user = _sqlDatabaseFixture.Db.Users.FindAll<Phone, Car, Phone>(x => x.Id == 1, q => q.OfficePhone, q => q.Cars, q => q.Phone).First();
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("333", user.OfficePhone.Number);
            Assert.Equal("123", user.Phone.Number);
        }

        [Fact]
        public async void FindAllJoinSameTableTwiceAsync()
        {
            var user = (await _sqlDatabaseFixture.Db.Users.FindAllAsync<Phone, Car, Phone>(x => x.Id == 1, q => q.OfficePhone, q => q.Cars, q => q.Phone)).First();
            Assert.True(user.Cars.Count == 1);
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
                var user = await _sqlDatabaseFixture.Db.Users.FindAsync(x => x.Id == id);
                Assert.False(user.Deleted);
                Assert.Equal(name, user.Name);
            }
            {
                var user = await _sqlDatabaseFixture.Db.Users.FindAsync(x => x.Name == name);
                Assert.Equal(id, user.Id);

                Assert.Null(user.Cars);
            }
        }

        [Fact]
        public void FindJoin()
        {
            var user = _sqlDatabaseFixture.Db.Users.Find<Car>(x => x.Id == 1, q => q.Cars);
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);

            var car = _sqlDatabaseFixture.Db.Cars.Find<User>(x => x.Id == 1, q => q.User);
            Assert.Equal("TestName0", car.User.Name);
        }

        [Fact]
        public async Task FindJoinAsync()
        {
            var user = await _sqlDatabaseFixture.Db.Users.FindAsync<Car>(x => x.Id == 1, q => q.Cars);
            Assert.True(user.Cars.Count == 1);
            Assert.Equal("TestCar0", user.Cars.First().Name);

            var car = await _sqlDatabaseFixture.Db.Cars.FindAsync<User>(x => x.Id == 1, q => q.User);
            Assert.Equal("TestName0", car.User.Name);
        }

        [Fact]
        public void InsertAndUpdate()
        {
            var user = new User
            {
                Name = "Sergey"
            };

            var insert =  _sqlDatabaseFixture.Db.Users.Insert(user);
            Assert.True(insert);

            var userFromDb =  _sqlDatabaseFixture.Db.Users.Find(q => q.Id == user.Id);
            Assert.Equal(user.Name, userFromDb.Name);
            user.Name = "Sergey1";

            var update =  _sqlDatabaseFixture.Db.Users.Update(user);
            Assert.True(update);
            userFromDb =  _sqlDatabaseFixture.Db.Users.Find(q => q.Id == user.Id);
            Assert.Equal("Sergey1", userFromDb.Name);
        }

        [Fact]
        public async Task InsertAndUpdateAsync()
        {
            var user = new User
            {
                Name = "Sergey"
            };

            var insert = await _sqlDatabaseFixture.Db.Users.InsertAsync(user);
            Assert.True(insert);

            var userFromDb = await _sqlDatabaseFixture.Db.Users.FindAsync(q => q.Id == user.Id);
            Assert.Equal(user.Name, userFromDb.Name);
            user.Name = "Sergey1";

            var update = await _sqlDatabaseFixture.Db.Users.UpdateAsync(user);
            Assert.True(update);
            userFromDb = await _sqlDatabaseFixture.Db.Users.FindAsync(q => q.Id == user.Id);
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

            var insert = _sqlDatabaseFixture.Db.Cars.Insert(car);
            Assert.True(insert);
            var carFromDb = _sqlDatabaseFixture.Db.Cars.Find(x => x.Id == car.Id);
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

            var insert = await _sqlDatabaseFixture.Db.Cars.InsertAsync(car);
            Assert.True(insert);
            var carFromDb = await _sqlDatabaseFixture.Db.Cars.FindAsync(x => x.Id == car.Id);
            var guid2 = new Guid(carFromDb.Data);
            Assert.Equal(guid, guid2);
        }

        [Fact]
        public async Task LogicalDeletedBoolAsync()
        {
            const int id = 10;

            var user = _sqlDatabaseFixture.Db.Users.Find(x => x.Id == id);
            Assert.False(user.Deleted);

            var deleted = await _sqlDatabaseFixture.Db.Users.DeleteAsync(user);
            Assert.True(deleted);

            var deletedUser = _sqlDatabaseFixture.Db.Users.Find(x => x.Id == id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task LogicalDeletedEnumAsync()
        {
            const int id = 1;

            var car = _sqlDatabaseFixture.Db.Cars.Find(x => x.Id == id);
            Assert.False(car.Status == StatusCar.Deleted);

            var deleted = await _sqlDatabaseFixture.Db.Cars.DeleteAsync(car);
            Assert.True(deleted);

            var deletedCar = _sqlDatabaseFixture.Db.Cars.Find(x => x.Id == id);
            Assert.Null(deletedCar);
        }

        [Fact]
        public async Task TransactionTest()
        {
            var user = new User
            {
                Name = "Sergey_Transaction"
            };
            using (var trans = _sqlDatabaseFixture.Db.BeginTransaction())
            {
                await _sqlDatabaseFixture.Db.Users.InsertAsync(user, trans);
                trans.Rollback();
            }
            var userFromDb = await _sqlDatabaseFixture.Db.Users.FindAsync(x => x.Name == "Sergey_Transaction");
            Assert.Null(userFromDb);

            using (var trans = _sqlDatabaseFixture.Db.BeginTransaction())
            {
                await _sqlDatabaseFixture.Db.Users.InsertAsync(user, trans);
                trans.Commit();
            }
            userFromDb = await _sqlDatabaseFixture.Db.Users.FindAsync(x => x.Name == "Sergey_Transaction");
            Assert.NotNull(userFromDb);
        }

        [Fact]
        public void FindAllJoinWithoutKeyTable()
        {
            try
            {
                var address = _sqlDatabaseFixture.Db.Address.FindAll<City>(x => x.Id == 1, q => q.City).First();
            }
            catch (NotSupportedException e)
            {
                Assert.Equal("Join doesn't support without [Key] attribute", e.Message);
            }
        }

        [Fact]
        public async Task FindAsyncJoinCompositeKey()
        {
            await _sqlDatabaseFixture.Db.Reports.InsertAsync(new Report
            {
                Id = 20,
                AnotherId = 20000,
                UserId = 1
            });

            var reportOne = await _sqlDatabaseFixture.Db.Reports.FindAsync<User>(x => x.Id == 20, q => q.User);
            Assert.Equal("TestName0", reportOne.User.Name);

            await _sqlDatabaseFixture.Db.Reports.InsertAsync(new Report
            {
                Id = 30,
                AnotherId = 20000,
                UserId = 1
            });

            var reportAll = (await _sqlDatabaseFixture.Db.Reports.FindAllAsync<User>(x => x.AnotherId == 20000, q => q.User)).ToArray();
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
                new Address { Street = "aaa0" , CityId = "10"},
                new Address { Street = "aaa1" , CityId = "11"}
            };

            int inserted = await _sqlDatabaseFixture.Db.Address.BulkInsertAsync(adresses);
            Assert.Equal(2, inserted);

            var adresses0 = await _sqlDatabaseFixture.Db.Address.FindAsync(x => x.CityId == "10");
            var adresses1 = await _sqlDatabaseFixture.Db.Address.FindAsync(x => x.CityId == "11");

            Assert.Equal("aaa0", adresses0.Street);
            Assert.Equal("aaa1", adresses1.Street);
        }

        [Fact]
        public void BulkInsert()
        {
            List<Address> adresses = new List<Address>
            {
                new Address { Street = "aaa0" , CityId = "10"},
                new Address { Street = "aaa1" , CityId = "11"}
            };

            int inserted = _sqlDatabaseFixture.Db.Address.BulkInsert(adresses);
            Assert.Equal(2, inserted);

            var adresses0 = _sqlDatabaseFixture.Db.Address.Find(x => x.CityId == "10");
            var adresses1 = _sqlDatabaseFixture.Db.Address.Find(x => x.CityId == "11");

            Assert.Equal("aaa0", adresses0.Street);
            Assert.Equal("aaa1", adresses1.Street);
        }

        [Fact]
        public void Delete()
        {
            List<Address> adresses = new List<Address>
            {
                new Address { Street = "aaa10" , CityId = "110"},
                new Address { Street = "aaa10" , CityId = "111"},
                new Address { Street = "aaa10" , CityId = "112"}
            };

            int inserted = _sqlDatabaseFixture.Db.Address.BulkInsert(adresses);
            Assert.Equal(3, inserted);

            var adresses0 = _sqlDatabaseFixture.Db.Address.Find(x => x.CityId == "110");
            var adresses1 = _sqlDatabaseFixture.Db.Address.Find(x => x.CityId == "111");
            var objectsCount = _sqlDatabaseFixture.Db.Address.FindAll(x => x.Street == "aaa10").Count();

            Assert.Equal(3, objectsCount);

            Assert.Equal("aaa10", adresses0.Street);
            Assert.Equal("aaa10", adresses1.Street);

            _sqlDatabaseFixture.Db.Address.Delete(x => x.Street == "aaa10" && x.CityId != "112");

            objectsCount = _sqlDatabaseFixture.Db.Address.FindAll(x => x.Street == "aaa10").Count();

            Assert.Equal(1, objectsCount);
        }

        [Fact]
        public void BulkUpdate()
        {
            var phone1 = new Phone { Code = "Kiev123", Number = "Kiev123" };
            var phone2 = new Phone { Code = "Kiev123", Number = "Kiev333" };

            _sqlDatabaseFixture.Db.Phones.Insert(phone1);
            _sqlDatabaseFixture.Db.Phones.Insert(phone2);

            var insertedPhone1 = _sqlDatabaseFixture.Db.Phones.FindById(phone1.Id);
            var insertedPhone2 = _sqlDatabaseFixture.Db.Phones.FindById(phone2.Id);
            Assert.Equal("Kiev123", phone1.Number);
            Assert.Equal("Kiev333", phone2.Number);

            insertedPhone1.Number = "Kiev666";
            insertedPhone2.Number = "Kiev777";

            bool result = _sqlDatabaseFixture.Db.Phones.BulkUpdate(new List<Phone> { insertedPhone1, insertedPhone2 });

            Assert.True(result);

            var newPhone1 = _sqlDatabaseFixture.Db.Phones.FindById(phone1.Id);
            var newPhone2 = _sqlDatabaseFixture.Db.Phones.FindById(phone2.Id);

            Assert.Equal("Kiev666", newPhone1.Number);
            Assert.Equal("Kiev777", newPhone2.Number);
        }

        [Fact]
        public async void BulkUpdateAsync()
        {
            var phone1 = new Phone { Code = "MSK123", Number = "MSK123" };
            var phone2 = new Phone { Code = "MSK123", Number = "MSK333" };

            await _sqlDatabaseFixture.Db.Phones.InsertAsync(phone1);
            await _sqlDatabaseFixture.Db.Phones.InsertAsync(phone2);

            var insertedPhone1 = await _sqlDatabaseFixture.Db.Phones.FindByIdAsync(phone1.Id);
            var insertedPhone2 = await _sqlDatabaseFixture.Db.Phones.FindByIdAsync(phone2.Id);
            Assert.Equal("MSK123", phone1.Number);
            Assert.Equal("MSK333", phone2.Number);

            insertedPhone1.Number = "MSK666";
            insertedPhone2.Number = "MSK777";

            bool result = await _sqlDatabaseFixture.Db.Phones.BulkUpdateAsync(new List<Phone> { insertedPhone1, insertedPhone2 });

            Assert.True(result);

            var newPhone1 = await _sqlDatabaseFixture.Db.Phones.FindByIdAsync(phone1.Id);
            var newPhone2 = await _sqlDatabaseFixture.Db.Phones.FindByIdAsync(phone2.Id);

            Assert.Equal("MSK666", newPhone1.Number);
            Assert.Equal("MSK777", newPhone2.Number);
        }


        [Fact]
        public async Task LogicalDeleteWithUpdatedAt()
        {
            const string name = "testLogicalDeleted1";
            var user = new User()
            {
                Name = name
            };

            await _sqlDatabaseFixture.Db.Users.InsertAsync(user);
            user = await _sqlDatabaseFixture.Db.Users.FindAsync(q => q.Name == name);

            //var updatedAt = user.UpdatedAt;

            await _sqlDatabaseFixture.Db.Users.DeleteAsync(user);
            user = await _sqlDatabaseFixture.Db.Users.FindAsync(q => q.Name == name);

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

            await _sqlDatabaseFixture.Db.Users.InsertAsync(user);
            user = await _sqlDatabaseFixture.Db.Users.FindAsync(q => q.Name == name);

            // var updatedAt = user.UpdatedAt;

            await _sqlDatabaseFixture.Db.Users.DeleteAsync(q => q.Id == user.Id);
            user = await _sqlDatabaseFixture.Db.Users.FindAsync(q => q.Name == name);

            Assert.Null(user);

        }

        [Fact]
        public void InsertAndUpdate_WithGuid_WithoutKey()
        {
            var identifier = Guid.NewGuid();
            var city = new City { Identifier = identifier, Name = "Moscow" };

            _sqlDatabaseFixture.Db.Cities.Insert(city);
            city = _sqlDatabaseFixture.Db.Cities.Find(q => q.Identifier == identifier);

            Assert.NotNull(city);
            city.Name = "Moscow1";
            _sqlDatabaseFixture.Db.Cities.Update(q => q.Identifier == identifier, city);

            city = _sqlDatabaseFixture.Db.Cities.Find(q => q.Identifier == identifier);
            Assert.NotNull(city);
            Assert.Equal("Moscow1", city.Name);
        }

        [Fact]
        public async Task InsertAndUpdate_WithGuid_WithoutKey_Async()
        {
            var identifier = Guid.NewGuid();
            var city = new City { Identifier = identifier, Name = "Moscow" };

            await _sqlDatabaseFixture.Db.Cities.InsertAsync(city);
            city = await _sqlDatabaseFixture.Db.Cities.FindAsync(q => q.Identifier == identifier);

            Assert.NotNull(city);
            city.Name = "Moscow1";
            await _sqlDatabaseFixture.Db.Cities.UpdateAsync(q => q.Identifier == identifier, city);

            city = await _sqlDatabaseFixture.Db.Cities.FindAsync(q => q.Identifier == identifier);
            Assert.NotNull(city);
            Assert.Equal("Moscow1", city.Name);
        }
    }
}