using System;
using System.Collections.Generic;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using MicroOrm.Dapper.Repositories.Tests.Classes;
using Xunit;

namespace MicroOrm.Dapper.Repositories.Tests.SqlGeneratorTests;

public class SqlGeneratorTests
{
    public class GetKeysParamTest
    {
        [Fact]
        public void KeyValuesLengthNotEqualKeysCount()
        {
            var sqlGenerator = new SqlGenerator<Report>();

            var ex = Assert.Throws<ArgumentException>(() => sqlGenerator.GetKeysParam(new[] { 1 }));

            Assert.Equal("id", ex.ParamName);
            Assert.StartsWith("GetSelectById id(Array) length not equals key properties count", ex.Message);
        }

        [Fact]
        public void KeyValuesContainsNull()
        {
            var sqlGenerator = new SqlGenerator<Report>();

            var ex = Assert.Throws<ArgumentException>(() => sqlGenerator.GetKeysParam(new object[] { 1, null }));

            Assert.Equal("id", ex.ParamName);
            Assert.StartsWith("Key value is null in 1", ex.Message);
        }

        [Fact]
        public void SingleKeyMustConvertToDictionary()
        {
            var sqlGenerator = new SqlGenerator<User>();

            var id = new { Id = 1 };

            var param = Assert.IsType<Dictionary<string, object>>(sqlGenerator.GetKeysParam(id));

            var keyValue = Assert.Single(param);

            Assert.Equal("Id", keyValue.Key);
            Assert.Equal(id, keyValue.Value);
        }

        [Fact]
        public void ArrayKeyValuesShouldConvertToDictionary()
        {
            var sqlGenerator = new SqlGenerator<Report>();

            var param = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object>>(sqlGenerator.GetKeysParam(new[] { 1, 2 }));

            Assert.Equal(2, param.Count);

            Assert.Equal(1, Assert.Contains("Id", param));
            Assert.Equal(2, Assert.Contains("AnotherId", param));
        }

        [Fact]
        public void ObjectKeyValuesShouldNotConvert()
        {
            var sqlGenerator = new SqlGenerator<Report>();

            var id = new { Id = 1, AnotherId = 2 };

            var param = sqlGenerator.GetKeysParam(id);

            Assert.Equal(id, param);
        }
    }
}
