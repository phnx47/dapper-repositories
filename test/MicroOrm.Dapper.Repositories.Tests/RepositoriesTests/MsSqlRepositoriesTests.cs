using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class MsSqlRepositoriesTests : RepositoriesTests, IClassFixture<MsSqlDatabaseFixture>
    {
        public MsSqlRepositoriesTests(MsSqlDatabaseFixture msSqlDatabaseFixture)
            : base(msSqlDatabaseFixture.Db)
        {
        }

        // only Databse specific tests
        
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
            DeleteCities();
            var cities = new List<City>();
            for (int i = 0; i < 17250; i++)
            {
                cities.Add(new City() { Identifier = Guid.NewGuid(), Name = $"City Number {i}" });
            }

            var inserted = Db.Cities.BulkInsert(cities);
            Assert.True(inserted.Equals(cities.Count));

            var random = new Random();
            var cityName = $"City Number {random.Next(17250)}";
            var randomcity = Db.Cities.Find(q => q.Name == cityName);
            Assert.NotNull(randomcity);
        }

        [Fact]
        public async Task BulkInsert_LotsOfCities_Async()
        {
            DeleteCities();
            var cities = new List<City>();
            for (int i = 0; i < 17250; i++)
            {
                cities.Add(new City() { Identifier = Guid.NewGuid(), Name = $"City Number {i}" });
            }

            var inserted = await Db.Cities.BulkInsertAsync(cities);
            Assert.True(inserted.Equals(cities.Count));

            var random = new Random();
            var cityName = $"City Number {random.Next(17250)}";
            var randomCity = Db.Cities.Find(q => q.Name == cityName);
            Assert.NotNull(randomCity);
        }

        [Fact]
        public async Task BulkInsertAndUpdate_LotsOfUsers_Async()
        {
            DeleteUsers();
            var users = new List<User>();
            for (int i = 0; i < 17250; i++)
            {
                users.Add(new User() { Name = $"User Number {i}", PhoneId = 1, OfficePhoneId = 2 });
            }

            var inserted = await Db.Users.BulkInsertAsync(users);
            Assert.True(inserted.Equals(users.Count));
            users.Clear();

            var random = new Random();
            var userName = $"User Number {random.Next(17250)}";
            var randomUser= Db.Users.Find(q => q.Name == userName);
            Assert.NotNull(randomUser);


            var updatedUsers = await Db.Connection.QueryAsync<User>("SELECT * FROM Users WHERE Name LIKE 'User Number%'");
            updatedUsers?.ToList().ForEach(x => x.Name += " Updated");
            var updated = await Db.Users.BulkUpdateAsync(updatedUsers);
            Assert.True(updated);

            updatedUsers = await Db.Connection.QueryAsync<User>("SELECT * FROM Users WHERE Name LIKE 'User Number%Updated'");
            Assert.True(updatedUsers.Count().Equals(17250));
        }

        [Fact]
        public void BulkInsertAndUpdate_LotsOfUsers()
        {
            DeleteUsers();
            var users = new List<User>();
            for (int i = 0; i < 17250; i++)
            {
                users.Add(new User() { Name = $"User Number {i}", PhoneId = 1, OfficePhoneId = 2 });
            }

            var inserted = Db.Users.BulkInsert(users);
            Assert.True(inserted.Equals(users.Count));
            users.Clear();

            var random = new Random();
            var userName = $"User Number {random.Next(17250)}";
            var randomUser = Db.Users.Find(q => q.Name == userName);
            Assert.NotNull(randomUser);


            var updatedUsers = Db.Connection.Query<User>("SELECT * FROM Users WHERE Name LIKE 'User Number%'");
            updatedUsers?.ToList().ForEach(x => x.Name += " Updated");
            var updated =  Db.Users.BulkUpdate(updatedUsers);
            Assert.True(updated);

            updatedUsers = Db.Connection.Query<User>("SELECT * FROM Users WHERE Name LIKE 'User Number%Updated'");
            Assert.True(updatedUsers.Count().Equals(17250));
        }

        private void DeleteCities() {
            var sql = "DELETE FROM Cities";
            Db.Connection.Execute(sql);
        }


        private  void DeleteUsers()
        {
            var sql = "DELETE FROM Users Where Name like 'User Number%'";
            Db.Connection.Execute(sql);
        }
        

        /*

            cities.FirstOrDefault().Name = "First City";
            cities.LastOrDefault().Name = "Last City";
            var updated = await Db.Cities.BulkUpdateAsync(cities);
            Assert.True(updated.Equals(cities.Count));

            var currentCity = Db.Cities.Find(q => q.Name == "First City");
            Assert.NotNull(currentCity);

            currentCity = Db.Cities.Find(q => q.Name == "Last City");
            Assert.NotNull(currentCity);         
         */
    }
}
