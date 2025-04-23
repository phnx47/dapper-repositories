using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Repositories.MySql.Tests;

public class MySqlConnectorRepositoriesTests(MySqlConnectorDatabaseFixture fixture) : RepositoriesTests<MySqlConnectorDatabaseFixture>(fixture)
{
    [Fact]
    public async Task CancellationTokenSource_Cancel()
    {
        using var cts = new CancellationTokenSource();

        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Db.Address.FindAllAsync(cts.Token));
    }
}
