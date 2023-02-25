namespace Repositories.MySql.Tests;

public class MySqlConnectorRepositoriesTests : RepositoriesTests<MySqlConnectorDatabaseFixture>
{
    public MySqlConnectorRepositoriesTests(MySqlConnectorDatabaseFixture fixture)
        : base(fixture)
    {
    }
}
