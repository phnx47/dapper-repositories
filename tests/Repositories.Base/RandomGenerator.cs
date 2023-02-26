using System;
using System.Text;

namespace Repositories.Base;

public static class RandomGenerator
{
    private const string _chars = "0123456789abcdefghijklmnopqrstuvwxyz";
    private static readonly Random _random = new();

    public static string String()
    {
        var output = new StringBuilder();

        for (int i = 0; i < 10; i++)
            output.Append(_chars[_random.Next(_chars.Length)]);

        return output.ToString();
    }
}
