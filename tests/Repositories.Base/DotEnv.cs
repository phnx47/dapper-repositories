using System;
using System.IO;

namespace Repositories.Base;

public static class DotEnv
{
    private const string _fileName = ".env";

    static DotEnv()
    {
        if (!File.Exists(_fileName))
            return;

        foreach (var line in File.ReadAllLines(_fileName))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }

    public static string GetTestDbPass() => Environment.GetEnvironmentVariable("TEST_DB_PASS");
}
