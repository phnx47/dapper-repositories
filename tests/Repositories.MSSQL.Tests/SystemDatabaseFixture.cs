using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Repositories.Base;

namespace Repositories.MSSQL.Tests;

public class SystemDatabaseFixture : DatabaseFixture
{
    public SystemDatabaseFixture()
        : base(new TestDbContext(new SqlConnection("Server=localhost;Database=master;User ID=sa;Password=Password12!"), SqlProvider.MSSQL))
    {
    }
}
