using System.Data.SQLite;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Repositories.Base;

namespace Repositories.SQLite.Tests;

public class SystemDatabaseFixture : DatabaseFixture
{
    public SystemDatabaseFixture()
        : base(new TestDbContext(new SQLiteConnection($"Data Source={RandomGenerator.String()};Mode=Memory;Cache=Shared"), SqlProvider.SQLite, true))
    {
    }
}
