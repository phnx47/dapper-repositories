using System.Data;
using System.Threading.Tasks;
using MicroOrm.Dapper.Repositories.DbContext;
using Xunit;

namespace Repositories.Base;

public abstract class BaseDbContextTests
{
    protected abstract IDapperDbContext CreateContext();

    [Fact]
    public void Connection_ReturnsOpenConnection()
    {
        using var context = CreateContext();

        var connection = context.Connection;

        Assert.NotNull(connection);
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void OpenConnection_WhenConnectionClosed_OpensConnection()
    {
        using var context = CreateContext();
        if (context.Connection.State != ConnectionState.Closed)
            context.Connection.Close();

        context.OpenConnection();

        Assert.Equal(ConnectionState.Open, context.Connection.State);
    }

    [Fact]
    public void OpenConnection_WhenConnectionAlreadyOpen_DoesNothing()
    {
        using var context = CreateContext();
        context.OpenConnection();

        context.OpenConnection();

        Assert.Equal(ConnectionState.Open, context.Connection.State);
    }

    [Fact]
    public void BeginTransaction_ReturnsValidTransaction()
    {
        using var context = CreateContext();

        using var transaction = context.BeginTransaction();

        Assert.NotNull(transaction);
        Assert.Equal(ConnectionState.Open, context.Connection.State);

        transaction.Rollback();
    }

    [Fact]
    public void Dispose_ClosesOpenConnection()
    {
        var context = CreateContext();
        var connection = context.Connection;
        Assert.Equal(ConnectionState.Open, connection.State);

        context.Dispose();

        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    [Fact]
    public void Dispose_DoesNothingIfConnectionAlreadyClosed()
    {
        var context = CreateContext();
        var connection = context.Connection;
        connection.Close();

        context.Dispose();

        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    [Fact]
    public async Task OpenConnectionAsync_WhenConnectionClosed_OpensConnection()
    {
        using var context = CreateContext();
        if (context.Connection.State != ConnectionState.Closed)
            context.Connection.Close();

        await context.OpenConnectionAsync();

        Assert.Equal(ConnectionState.Open, context.Connection.State);
    }

    [Fact]
    public async Task OpenConnectionAsync_WhenConnectionAlreadyOpen_DoesNothing()
    {
        using var context = CreateContext();
        await context.OpenConnectionAsync();

        await context.OpenConnectionAsync();

        Assert.Equal(ConnectionState.Open, context.Connection.State);
    }

    [Fact]
    public async Task BeginTransactionAsync_ReturnsValidTransaction()
    {
        using var context = CreateContext();

        using var transaction = await context.BeginTransactionAsync();

        Assert.NotNull(transaction);
        Assert.Equal(ConnectionState.Open, context.Connection.State);

        transaction.Rollback();
    }

    [Fact]
    public async Task BeginTransactionAsync_AutomaticallyOpensConnection()
    {
        using var context = CreateContext();
        if (context.Connection.State != ConnectionState.Closed)
            context.Connection.Close();

        using var transaction = await context.BeginTransactionAsync();

        Assert.NotNull(transaction);
        Assert.Equal(ConnectionState.Open, context.Connection.State);

        transaction.Rollback();
    }

    [Fact]
    public async Task ConnectionAndTransactionOperations_WorkTogether()
    {
        using var context = CreateContext();

        var connection = context.Connection;
        Assert.Equal(ConnectionState.Open, connection.State);

        using var transaction = await context.BeginTransactionAsync();
        Assert.NotNull(transaction);

        connection.Close();
        Assert.Equal(ConnectionState.Closed, connection.State);

        await context.OpenConnectionAsync();
        Assert.Equal(ConnectionState.Open, connection.State);

        using var transaction2 = context.BeginTransaction();
        Assert.NotNull(transaction2);
    }
}
