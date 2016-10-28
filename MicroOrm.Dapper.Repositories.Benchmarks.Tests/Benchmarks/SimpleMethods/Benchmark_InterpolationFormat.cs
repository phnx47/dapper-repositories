using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace MicroOrm.Dapper.Repositories.Benchmarks
{
    public class Benchmark_InterpolationFormat
    {
        private const string TableName = "Name";
        private const string StatusPropertyName = "StatusPropertyName";
        private const string LogicalDeleteValue = "LogicalDeleteValue";

        [Benchmark]
        public void Interpolation()
        {
            var sql = $"UPDATE {TableName} SET {StatusPropertyName} = {LogicalDeleteValue}";
        }

        [Benchmark]
        public void Format()
        {
            var sql = string.Format("UPDATE {0} SET {1} = {2}", TableName, StatusPropertyName, LogicalDeleteValue);
        }

        [Benchmark]
        public void Plus()
        {
            var sql = "UPDATE " + TableName + " SET " + StatusPropertyName + " = " + LogicalDeleteValue;
        }


    }
}
