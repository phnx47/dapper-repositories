using BenchmarkDotNet.Running;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Benchmarks;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark_Generator_GetSelectFirst>();
            //BenchmarkRunner.Run<Benchmark_Generator_GetSelectFirst_Connector>();
        }
    }
}