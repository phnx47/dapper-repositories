using MicroOrm.Dapper.Repositories.SqlGenerator;
using MySql.Data.MySqlClient;
using Repositories.Base;

namespace Repositories.MySQL.Tests;

public class MySqlClientDatabaseFixture()
    : DatabaseFixture(new TestDbContext(new MySqlConnection($"Server=localhost;Uid=root;Pwd={DotEnv.GetTestDbPass()}"), SqlProvider.MySQL));
