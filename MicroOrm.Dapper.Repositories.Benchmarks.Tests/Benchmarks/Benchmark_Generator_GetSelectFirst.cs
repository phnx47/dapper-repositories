using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Classes;
using MicroOrm.Dapper.Repositories.SqlGenerator;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests.Benchmarks
{
    [Config(typeof(BenchmarkDotNetConfig))]
    public class Benchmark_Generator_GetSelectFirst
    {
        [Benchmark]
        public void GetSelectFirst()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);
            sqlGenerator.GetSelectFirst(null);
        }

        [Benchmark]
        public void GetSelectFirstPredicate()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);
            sqlGenerator.GetSelectFirst(q => q.Id == 2);
        }

        [Benchmark]
        public void GetSelectFirstPredicateJoin()
        {
            ISqlGenerator<User> sqlGenerator = new SqlGenerator<User>(ESqlConnector.MSSQL);
            sqlGenerator.GetSelectFirst(q => q.Id == 2, q => q.Cars);
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