using System.Linq;
using BenchmarkDotNet.Running;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Benchmarks;
using MicroOrm.Dapper.Repositories.Benchmarks.Tests.Orm;
using Microsoft.EntityFrameworkCore;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark_Repository_FindAll>();

            //BenchmarkRunner.Run<Benchmark_Generator_GetSelectFirst>();
            //BenchmarkRunner.Run<Benchmark_Generator_GetSelectFirst_Connector>();
        }
    }
}