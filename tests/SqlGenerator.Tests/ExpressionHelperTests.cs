using System;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using TestClasses;
using Xunit;

namespace SqlGenerator.Tests;

public class ExpressionHelperTests
{
    [Fact]
    public void GetMethodCallSqlOperator_Contains()
    {
        var result = ExpressionHelper.GetMethodCallSqlOperator("Contains");
        Assert.Equal("IN", result);
    }

    [Fact]
    public void GetMethodCallSqlOperator_NotContains()
    {
        var result = ExpressionHelper.GetMethodCallSqlOperator("Contains", true);
        Assert.Equal("NOT IN", result);
    }

    [Fact]
    public void GetMethodCallSqlOperator_Any()
    {
        var result = ExpressionHelper.GetMethodCallSqlOperator("Any");
        Assert.Equal("ANY", result);
    }

    [Fact]
    public void GetMethodCallSqlOperator_All()
    {
        var result = ExpressionHelper.GetMethodCallSqlOperator("All");
        Assert.Equal("ALL", result);
    }

    [Fact]
    public void GetMethodCallSqlOperator_all()
    {
        Assert.Throws<NotSupportedException>(() => ExpressionHelper.GetMethodCallSqlOperator("all"));
    }

    [Fact]
    public void GetMethodCallSqlOperator_Like()
    {
        var result1 = ExpressionHelper.GetMethodCallSqlOperator("StartsWith");
        Assert.Equal("LIKE", result1);

        var result2 = ExpressionHelper.GetMethodCallSqlOperator("StringContains", true);
        Assert.Equal("NOT LIKE", result2);

        var result3 = ExpressionHelper.GetMethodCallSqlOperator("EndsWith");
        Assert.Equal("LIKE", result3);
    }

    [Fact]
    public void GetSqlLikeValue_Like()
    {
        var result1 = ExpressionHelper.GetSqlLikeValue("StartsWith", "123");
        Assert.Equal("123%", result1);

        var result2 = ExpressionHelper.GetSqlLikeValue("StringContains", "456");
        Assert.Equal("%456%", result2);

        var result3 = ExpressionHelper.GetSqlLikeValue("EndsWith", 789);
        Assert.Equal("%789", result3);
    }

    [Fact]
    public void IsIgnoreCase()
    {
        Assert.True(ExpressionHelper.IsIgnoreCase(Call(x => x.Name.Contains("ab", StringComparison.OrdinalIgnoreCase))));
        Assert.True(ExpressionHelper.IsIgnoreCase(Call(x => x.Name.StartsWith("ab", StringComparison.InvariantCultureIgnoreCase))));
        Assert.True(ExpressionHelper.IsIgnoreCase(Call(x => x.Name.Equals("ab", StringComparison.CurrentCultureIgnoreCase))));

        var comparison = StringComparison.OrdinalIgnoreCase;
        Assert.True(ExpressionHelper.IsIgnoreCase(Call(x => x.Name.EndsWith("ab", comparison))));

        Assert.False(ExpressionHelper.IsIgnoreCase(Call(x => x.Name.Contains("ab"))));
        Assert.False(ExpressionHelper.IsIgnoreCase(Call(x => x.Name.StartsWith("ab", StringComparison.Ordinal))));
        Assert.False(ExpressionHelper.IsIgnoreCase(Call(x => x.Name.Equals("ab", StringComparison.InvariantCulture))));
    }

    private static MethodCallExpression Call(Expression<Func<User, bool>> predicate)
    {
        return (MethodCallExpression)predicate.Body;
    }
}
