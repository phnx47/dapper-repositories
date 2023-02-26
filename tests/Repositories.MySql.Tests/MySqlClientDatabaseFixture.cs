using MicroOrm.Dapper.Repositories.SqlGenerator;
using MySql.Data.MySqlClient;
using Repositories.Base;

namespace Repositories.MySql.Tests;

public class MySqlClientDatabaseFixture : DatabaseFixture
{
    public MySqlClientDatabaseFixture()
        : base( new TestDbContext(new MySqlConnection("Server=localhost;Uid=root;Pwd=Password12!"), SqlProvider.MySQL))
    {
    }
}
