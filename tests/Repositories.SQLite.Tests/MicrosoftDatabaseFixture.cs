using MicroOrm.Dapper.Repositories.SqlGenerator;
using Microsoft.Data.Sqlite;
using Repositories.Base;

namespace Repositories.SQLite.Tests;

public class MicrosoftDatabaseFixture : DatabaseFixture
{
    public MicrosoftDatabaseFixture()
        : base(new TestDbContext(new SqliteConnection($"Data Source={RandomGenerator.String()};Mode=Memory;Cache=Shared"), SqlProvider.SQLite, true))
    {
    }
}
