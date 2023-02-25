using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using MicroOrm.Dapper.Repositories.Tests.DatabaseFixture;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.RepositoriesTests
{
    public class PostgresRepositoriesTests : RepositoriesTests, IClassFixture<PostgresDatabaseFixture>
    {
        public PostgresRepositoriesTests(PostgresDatabaseFixture msSqlDatabaseFixture)
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
}
