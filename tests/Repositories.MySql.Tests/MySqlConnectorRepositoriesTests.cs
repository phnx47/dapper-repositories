using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Repositories.MySql.Tests;

public class MySqlConnectorRepositoriesTests : RepositoriesTests<MySqlConnectorDatabaseFixture>
{
    public MySqlConnectorRepositoriesTests(MySqlConnectorDatabaseFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CancellationTokenSource_Cancel()
    {
        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Db.Address.FindAllAsync(cts.Token));
    }
}
