using System;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.OtherTests
{
    public class ExpressionHelperTests
    {
        [Fact]
        public void GetMethodCallSqlOperator_Contains()
        {
            var result = ExpressionHelper.GetMethodCallSqlOperator("Contains");
            Assert.Equal("IN", result);
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
    }
}