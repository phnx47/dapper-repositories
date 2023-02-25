namespace Repositories.MSSQL.Tests;

public class SystemRepositoriesTests : RepositoriesTests<SystemDatabaseFixture>
{
    public SystemRepositoriesTests(SystemDatabaseFixture fixture)
        : base(fixture)
    {
    }
}
