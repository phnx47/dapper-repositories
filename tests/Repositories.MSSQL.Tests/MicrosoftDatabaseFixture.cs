using MicroOrm.Dapper.Repositories.SqlGenerator;
using Microsoft.Data.SqlClient;
using Repositories.Base;

namespace Repositories.MSSQL.Tests;

public class MicrosoftDatabaseFixture : DatabaseFixture
{
    public MicrosoftDatabaseFixture()
        : base(new TestDbContext(
            new SqlConnection($"Server=localhost;Database=master;User ID=sa;Password={DotEnv.GetTestDbPass()};Trust Server Certificate=true"), SqlProvider.MSSQL))
    {
    }
}
