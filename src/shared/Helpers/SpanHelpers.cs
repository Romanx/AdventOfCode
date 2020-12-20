using System;

namespace Shared.Helpers
{
    public static class SpanHelpers
    {
        public static Span<ReadOnlyMemory<char>> SliceUntilBlankLine(Span<ReadOnlyMemory<char>> lines, out Span<ReadOnlyMemory<char>> rest)
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

            rest = Span<ReadOnlyMemory<char>>.Empty;
            return lines;
        }

        public static Memory<ReadOnlyMemory<char>> SliceUntilBlankLine(Memory<ReadOnlyMemory<char>> lines, out Memory<ReadOnlyMemory<char>> rest)
        {
            var index = 0;
            foreach (var line in lines.Span)
            {
                if (line.IsEmpty)
                {
                    rest = lines[(index + 1)..];
                    return lines[..index];
                }
                index++;
            }

            rest = Memory<ReadOnlyMemory<char>>.Empty;
            return lines;
        }

        public static char[,] As2dArray(Memory<ReadOnlyMemory<char>> lines)
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
