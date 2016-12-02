using BenchmarkDotNet.Running;
using MicroOrm.Dapper.Repositories.Benchmarks.Benchmarks.SimpleMethods;

namespace MicroOrm.Dapper.Repositories.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark_InterpolationFormat>();
           // BenchmarkRunner.Run<Benchmark_TableName>();
        }
    }
}