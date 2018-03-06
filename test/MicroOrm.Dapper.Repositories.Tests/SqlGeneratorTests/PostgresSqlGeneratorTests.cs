using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.SqlGeneratorTests
{
    public class PostgresSqlGeneratorTests
    {
        private const SqlProvider SqlConnector = SqlProvider.PostgreSQL;

        [Fact]
        public static void Count()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(SqlConnector, true);
            var sqlQuery = userSqlGenerator.GetCount(null);
            Assert.Equal("SELECT COUNT(*) FROM \"Users\" WHERE \"Users\".\"Deleted\" != 1", sqlQuery.GetSql());
        }

        [Fact]
        public static void CountWithDistinct()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(SqlConnector, true);
            var sqlQuery = userSqlGenerator.GetCount(null, user => user.AddressId);
            Assert.Equal("SELECT COUNT(DISTINCT \"Users\".\"AddressId\") FROM \"Users\" WHERE \"Users\".\"Deleted\" != 1", sqlQuery.GetSql());
        }

        [Fact]
        public static void CountWithDistinctAndWhere()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(SqlConnector, true);
            var sqlQuery = userSqlGenerator.GetCount(x => x.PhoneId == 1, user => user.AddressId);
            Assert.Equal("SELECT COUNT(DISTINCT \"Users\".\"AddressId\") FROM \"Users\" WHERE \"Users\".\"PhoneId\" = @PhoneId AND \"Users\".\"Deleted\" != 1", sqlQuery.GetSql());
        }

        [Fact]
        public void SelectFirst()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(SqlConnector);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty);
            Assert.Equal("SELECT Cities.Identifier, Cities.Name FROM Cities WHERE Cities.Identifier = @Identifier LIMIT 1", sqlQuery.GetSql());
        }


        [Fact]
        public void SelectFirst_QuoMarks()
        {
            ISqlGenerator<City> sqlGenerator = new SqlGenerator<City>(SqlConnector, true);
            var sqlQuery = sqlGenerator.GetSelectFirst(x => x.Identifier == Guid.Empty);
            Assert.Equal("SELECT \"Cities\".\"Identifier\", \"Cities\".\"Name\" FROM \"Cities\" WHERE \"Cities\".\"Identifier\" = @Identifier LIMIT 1", sqlQuery.GetSql());
        }
               
        [Fact]
        public static void BulkUpdate()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(SqlConnector);
            var phones = new List<Phone>
            {
                new Phone { Id = 10, IsActive = true, Number = "111" },
                new Phone { Id = 10, IsActive = false, Number = "222" }
            };

            var sqlQuery = userSqlGenerator.GetBulkUpdate(phones);

            Assert.Equal("UPDATE DAB.Phones SET Number = @Number0, IsActive = @IsActive0 WHERE Id = @Id0; " +
                         "UPDATE DAB.Phones SET Number = @Number1, IsActive = @IsActive1 WHERE Id = @Id1", sqlQuery.GetSql());
        }
        
        [Fact]
        public static void BulkUpdate_QuoMarks()
        {
            ISqlGenerator<Phone> userSqlGenerator = new SqlGenerator<Phone>(SqlConnector, true);
            var phones = new List<Phone>
            {
                new Phone { Id = 10, IsActive = true, Number = "111" },
                new Phone { Id = 10, IsActive = false, Number = "222" }
            };

            var sqlQuery = userSqlGenerator.GetBulkUpdate(phones);

            Assert.Equal("UPDATE \"DAB\".\"Phones\" SET \"Number\" = @Number0, \"IsActive\" = @IsActive0 WHERE \"Id\" = @Id0; " +
                         "UPDATE \"DAB\".\"Phones\" SET \"Number\" = @Number1, \"IsActive\" = @IsActive1 WHERE \"Id\" = @Id1", sqlQuery.GetSql());
        }
    }
}