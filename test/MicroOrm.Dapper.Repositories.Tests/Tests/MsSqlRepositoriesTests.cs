using System;
using System.Linq;
using System.Threading.Tasks;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.Tests
{
    public class MsSqlRepositoriesTests : IClassFixture<MsSqlDatabaseFixture>
    {
        public MsSqlRepositoriesTests(MsSqlDatabaseFixture msSqlDatabaseFixture)
        {
            _sqlDatabaseFixture = msSqlDatabaseFixture;
        }

        private readonly MsSqlDatabaseFixture _sqlDatabaseFixture;

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
        public void Find()
        {
            var user = _sqlDatabaseFixture.Db.Users.Find(x => x.Id == 2);
            Assert.False(user.Deleted);
            Assert.Equal("TestName1", user.Name);
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
        public void FindAllJoin2Table()
        {
            var user = _sqlDatabaseFixture.Db.Users.FindAll<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses).First();
            Assert.Equal(1, user.Cars.Count);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
        }

        [Fact]
        public async Task FindAllJoin2TableAsync()
        {
            var user = (await _sqlDatabaseFixture.Db.Users.FindAllAsync<Car, Address>(x => x.Id == 1, q => q.Cars, q => q.Addresses)).First();
            Assert.Equal(1, user.Cars.Count);
            Assert.Equal("TestCar0", user.Cars.First().Name);
            Assert.Equal("Street0", user.Addresses.Street);
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
            Assert.Equal(1, user.Cars.Count);
            Assert.Equal("TestCar0", user.Cars.First().Name);

            var car = _sqlDatabaseFixture.Db.Cars.Find<User>(x => x.Id == 1, q => q.User);
            Assert.Equal("TestName0", car.User.Name);
        }

        [Fact]
        public async Task FindJoinAsync()
        {
            var user = await _sqlDatabaseFixture.Db.Users.FindAsync<Car>(x => x.Id == 1, q => q.Cars);
            Assert.Equal(1, user.Cars.Count);
            Assert.Equal("TestCar0", user.Cars.First().Name);

            var car = await _sqlDatabaseFixture.Db.Cars.FindAsync<User>(x => x.Id == 1, q => q.User);
            Assert.Equal("TestName0", car.User.Name);
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
    }
}