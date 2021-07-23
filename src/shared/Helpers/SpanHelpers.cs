using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Shared.Helpers
{
    public static class SpanHelpers
    {
        public static ReadOnlySpan<ReadOnlyMemory<char>> SliceUntilBlankLine(ReadOnlySpan<ReadOnlyMemory<char>> lines, out ReadOnlySpan<ReadOnlyMemory<char>> rest)
        {
            var index = 0;
            foreach (var line in lines)
            {
                if (line.IsEmpty)
                {
                    rest = lines[(index + 1)..];
                    return lines[..index];
                }
                index++;
            }

            rest = ReadOnlySpan<ReadOnlyMemory<char>>.Empty;
            return lines;
        }

        public static ReadOnlyMemory<ReadOnlyMemory<char>> SliceUntilBlankLine(ReadOnlyMemory<ReadOnlyMemory<char>> lines, out ReadOnlyMemory<ReadOnlyMemory<char>> rest)
        {
            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines.Span[index];
                if (line.IsEmpty)
                {
                    var content = lines[..index];
                    while (lines.Span[index + 1].IsEmpty)
                    {
                        index++;
                    }
                    rest = lines[(index + 1)..];

                    return content;
                }
            }

            rest = ReadOnlyMemory<ReadOnlyMemory<char>>.Empty;
            return lines;
        }

        public static IEnumerable<ReadOnlyMemory<ReadOnlyMemory<char>>> SplitByBlankLines(ReadOnlyMemory<ReadOnlyMemory<char>> lines)
        {
            while (lines.IsEmpty is false)
            {
                var slice = SliceUntilBlankLine(lines, out lines);
                yield return slice;
            }
        }

        public static ImmutableArray<int> ParseCommaSeparatedList(ReadOnlySpan<char> span)
        {
            var builder = ImmutableArray.CreateBuilder<int>();

            while (span.IsEmpty is false)
            {
                var index = span.IndexOf(',');

                if (index == -1)
                {
                    builder.Add(int.Parse(span));
                    span = ReadOnlySpan<char>.Empty;
                }
                else
                {
                    var number = span.Slice(0, index);
                    span = span[(index + 1)..];
                    builder.Add(int.Parse(number));
                }
            }

            return builder.ToImmutable();
        }

        public static char[,] As2dArray(ReadOnlyMemory<ReadOnlyMemory<char>> lines)
        {
            var lineSpan = lines.Span;

            var array = new char[lineSpan[0].Length, lines.Length];

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lineSpan[y].Span;
                for (var x = 0; x < line.Length; x++)
                {
                    array[y, x] = line[x];
                }
            }

            return array;
        }
    }
}
