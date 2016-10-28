using BenchmarkDotNet.Attributes;

namespace MicroOrm.Dapper.Repositories.Benchmarks
{
    public class Benchmark_TableName
    {
        [Benchmark]
        public void InsertSymblos()
        {
            var tableName = "Name";

            tableName = tableName.Insert(0, "[");
            tableName = tableName.Insert(tableName.Length, "]");
        }

        [Benchmark]
        public void Interpolation()
        {
            var tableName = "Name";
            tableName = $"[{tableName}]";
        }

        [Benchmark]
        public void Plus()
        {
            var tableName = "Name";
            tableName = "[" + tableName + "]";
        }
    }

   
}