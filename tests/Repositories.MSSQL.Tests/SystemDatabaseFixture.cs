using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Repositories.Base;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Repositories.MSSQL.Tests;

public class SystemDatabaseFixture : DatabaseFixture
{
    public SystemDatabaseFixture()
        : base(new TestDbContext(new SqlConnection($"Server=localhost;Database=master;User ID=sa;Password={DotEnv.GetTestDbPass()}"), SqlProvider.MSSQL))
    {
    }
}
