using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace MicroOrm.Dapper.Repositories.Benchmarks
{
    public class Config : ManualConfig
    {
        public Config()
        {
            IJob job = new Job
            {
                TargetCount = 5,
                WarmupCount = 3,
                LaunchCount = 2,
                Runtime = Runtime.Core,
                Jit = Jit.RyuJit,
                Platform = Platform.X64
            };

            Add(job);
        }
    }
}
