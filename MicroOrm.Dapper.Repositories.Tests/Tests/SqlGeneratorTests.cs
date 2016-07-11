using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.Tests
{
    public class SqlGeneratorTests
    {

        public SqlGeneratorTests()
        {

        }


        [Fact]
        public void MSSQLSelectFirst()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 2);
            Assert.Equal(sqlQuery.Sql, "SELECT TOP 1 [Users].[Id], [Users].[Name], [Users].[Deleted] FROM [Users] WHERE [Users].[Id] = @Id AND [Users].[Deleted] != 1");
        }

        [Fact]
        public void MySQLSelectFirst()
        {
            ISqlGenerator<User> userSqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL);
            var sqlQuery = userSqlGenerator.GetSelectFirst(x => x.Id == 6);
            Assert.Equal(sqlQuery.Sql, "SELECT Users.Id, Users.Name, Users.Deleted FROM Users WHERE Users.Id = @Id AND Users.Deleted != 1 LIMIT 1");
        }
    }
}
