namespace Repositories.MySql.Tests;

public class MySqlClientRepositoriesTests : RepositoriesTests<MySqlClientDatabaseFixture>
{
    public MySqlClientRepositoriesTests(MySqlClientDatabaseFixture fixture)
        : base(fixture)
    {
    }
}
