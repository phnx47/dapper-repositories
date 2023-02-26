using MicroOrm.Dapper.Repositories.SqlGenerator;
using MySqlConnector;
using Repositories.Base;

namespace Repositories.MySql.Tests;

public class MySqlConnectorDatabaseFixture : DatabaseFixture
{
    public MySqlConnectorDatabaseFixture()
        : base(new TestDbContext(new MySqlConnection("Server=localhost;Uid=root;Pwd=Password12!"), SqlProvider.MySQL))
    {
    }
}
