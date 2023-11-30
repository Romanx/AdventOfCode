using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DayTwentyFive2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 25), "Full of Hot Air");

    public void PartOne(IInput input, IOutput output)
    {
        var total = input.Lines
            .Transform(SnafuNumber.FromSnafuNumber)
            .Sum();

        output.WriteProperty("Total", total);
        output.WriteProperty("Total (SNAFU)", SnafuNumber.ToSnafuNumber(total));
    }
}

internal static class ParseExtensions
{
}

public static class SnafuNumber
{
    public static long FromSnafuNumber(string s)
        => FromSnafuNumber(s.AsSpan());

    public static long FromSnafuNumber(ReadOnlySpan<char> s)
    {
        var ones = ConvertChar(s[^1]);
        long val = ones;

        for (var i = s.Length - 2; i >= 0; i--)
        {
            var c = s[i];
            var position = (s.Length - 1) - i;
            var multiplier = (long)Math.Pow(5, position);

            var converted = ConvertChar(c);

            val += converted * multiplier;
        }

        return val;

        static sbyte ConvertChar(char c) => c switch
        {
            '2' => 2,
            '1' => 1,
            '0' => 0,
            '-' => -1,
            '=' => -2,
            _ => throw new InvalidOperationException("Unable to work out conversion"),
        };
    }

    public static string ToSnafuNumber(long value)
    {
        var builder = new StringBuilder();

        while (value > 0)
        {
            var next = (value % 5) switch
            {
                0 => '0',
                1 => '1',
                2 => '2',
                3 => '=',
                4 => '-',
                _ => throw new UnreachableException()
            };
            builder.Insert(0, next);

            // If the remainder of n is 3 or higher then this will add a carry digit to account
            // for the subtraction.
            value = (value + 2) / 5;
        }

        return builder.ToString();
    }
}
