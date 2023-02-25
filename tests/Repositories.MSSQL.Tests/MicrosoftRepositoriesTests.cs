namespace Repositories.MSSQL.Tests;

public class MicrosoftRepositoriesTests : RepositoriesTests<MicrosoftDatabaseFixture>
{
    public MicrosoftRepositoriesTests(MicrosoftDatabaseFixture fixture)
        : base(fixture)
    {
    }
}
