using System;
using System.Collections.Immutable;

namespace Helpers.Computer
{
    public static class IntcodeParser
    {
        private static readonly char comma = ',';

        public static ImmutableArray<long> Parse(ReadOnlySpan<char> input)
        {
            var builder = ImmutableArray.CreateBuilder<long>();

            while (true)
            {
                var index = input.IndexOf(comma);
                if (index == -1)
                {
                    builder.Add(long.Parse(input));
                    break;
                }

                var slice = input.Slice(0, index);
                builder.Add(long.Parse(slice));
                input = input[(index + 1)..];
            }

            return builder.ToImmutable();
        }
    }
}
