using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Classes;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests.Benchmarks
{
    [Config(typeof(BenchmarkDotNetConfig))]
    public class Benchmark_Generator_GetSelectFirst_Connector
    {
        [Benchmark]
        public void GetSelectFirstMsSql()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);
            sqlGenerator.GetSelectFirst(null);
        }

        [Benchmark]
        public void GetSelectFirstMySql()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(ESqlConnector.MySQL);
            sqlGenerator.GetSelectFirst(null);
        }

        [Benchmark]
        public void GetSelectFirstPostgreSql()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(ESqlConnector.PostgreSQL);
            sqlGenerator.GetSelectFirst(null);
        }

        private class BenchmarkDotNetConfig : ManualConfig
        {
            public BenchmarkDotNetConfig()
            {
                var job = new Job();
                Add(job);
            }
        }
    }
}