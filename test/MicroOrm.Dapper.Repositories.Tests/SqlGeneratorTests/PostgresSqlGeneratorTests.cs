using System;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.SqlGeneratorTests
{
    public class PostgresSqlGeneratorTests
    {
        private const ESqlConnector SqlConnector = ESqlConnector.PostgreSQL;

        [Fact]
        public void SelectFirst()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(SqlConnector);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier LIMIT 1", sqlQuery.GetSql());
        }


        [Fact]
        public void SelectFirstQuoMarks()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(SqlConnector, true);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty);
            Assert.Equal("SELECT \"Cities\".\"Identifier\", \"Cities\".\"Name\" FROM \"Cities\" WHERE \"Cities\".\"Identifier\" = @Identifier LIMIT 1", sqlQuery.GetSql());
        }
    }
}