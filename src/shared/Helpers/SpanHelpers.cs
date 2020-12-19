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
    }
}
