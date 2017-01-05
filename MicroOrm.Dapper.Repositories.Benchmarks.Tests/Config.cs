using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace MicroOrm.Dapper.Repositories.Benchmarks
{
    public class Config : ManualConfig
    {
        public Config()
        {
            var job = new Job
            {
                Run =
                {
                    TargetCount = 5,
                    WarmupCount = 3,
                    LaunchCount = 2,
                },
                Env =
                {
                    Platform = Platform.X64,
                    Jit = Jit.RyuJit,
                    Runtime = Runtime.Core
               }
            };

            Add(job);
        }
    }
}