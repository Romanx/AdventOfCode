using Microsoft.Toolkit.HighPerformance;

namespace Helpers.Computer
{
    public static class IntcodeParser
    {
        private static readonly char comma = ',';

        public static ImmutableArray<long> Parse(ReadOnlySpan<char> input)
        {
            var builder = ImmutableArray.CreateBuilder<long>();

            foreach (var segment in input.Tokenize(comma))
            {
                builder.Add(long.Parse(segment));
            }

            return builder.ToImmutable();
        }
    }
}
