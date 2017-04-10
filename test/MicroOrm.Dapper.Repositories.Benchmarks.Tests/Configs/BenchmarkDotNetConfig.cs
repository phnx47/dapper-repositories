using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace MicroOrm.Dapper.Repositories.Benchmarks.Tests.Configs
{
    public class BenchmarkDotNetConfig : ManualConfig
    {
        public BenchmarkDotNetConfig()
        {
            var job = new Job();
            job.Env.Jit = Jit.RyuJit;
            job.Env.Platform = Platform.X64;
            job.Env.Runtime = Runtime.Core;
            Add(job);
        }
    }
}
