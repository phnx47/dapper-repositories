using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.SqlGeneratorTests
{
    public class SQLiteGeneratorTests
    {
        private const SqlProvider _sqlConnector = SqlProvider.SQLite;

        [Fact]
        public void Count()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(_sqlConnector, true);
            var sqlQuery = userSqlGenerator.GetCount(null);
            Assert.Equal("SELECT COUNT(*) FROM Users WHERE Users.Deleted != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectOrderBy()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var filterData = new FilterData();
            var data = filterData.OrderInfo ?? new OrderInfo();
            data.Columns = new List<string> { "Name" };
            data.Direction = OrderInfo.SortDirection.ASC;
            filterData.OrderInfo = data;

            var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 ORDER BY Name ASC", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectPaged()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var filterData = new FilterData();
            var data = filterData.LimitInfo ?? new LimitInfo();
            data.Limit = 10u;
            data.Offset = 5u;
            filterData.LimitInfo = data;

            var sqlQuery = sqlGenerator.GetSelectAll(x => x.Identifier == Guid.Empty, filterData);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 LIMIT 10 OFFSET 5", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectFirst2()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(_sqlConnector, false);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty && x.Name == "", null);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier_p0 AND Cities.Name = @Name_p1 LIMIT 1", sqlQuery.GetSql());
        }
    }
}
