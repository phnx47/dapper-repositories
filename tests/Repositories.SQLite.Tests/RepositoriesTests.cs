using Repositories.Base;
using Xunit;

namespace Repositories.SQLite.Tests;

public abstract class RepositoriesTests<TFixture> : BaseRepositoriesTests, IClassFixture<TFixture>
    where TFixture : DatabaseFixture
{
    protected RepositoriesTests(TFixture fixture)
        : base(fixture.Db)
    {
    }
}
