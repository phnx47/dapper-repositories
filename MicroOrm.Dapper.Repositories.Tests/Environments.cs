using System;

namespace MicroOrm.Dapper.Repositories.Tests
{
    internal static class Environments
    {
        public static readonly bool IsAppVeyor = Environment.GetEnvironmentVariable("Appveyor")?.ToUpperInvariant() == "TRUE";
    }
}