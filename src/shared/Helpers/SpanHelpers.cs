using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.HighPerformance;

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

        public static ReadOnlyMemory<ReadOnlyMemory<char>> SliceUntilCondition(
            ReadOnlyMemory<ReadOnlyMemory<char>> lines,
            Func<ReadOnlyMemory<char>, bool> condition,
            bool includeLine,
            out ReadOnlyMemory<ReadOnlyMemory<char>> rest)
        {
            for (var index = 0; index < lines.Length; index++)
            {
                if (index is 0 && condition(lines.Span[0]))
                {
                    index++;
                }

                var line = lines.Span[index];
                if (condition(line))
                {
                    var content = lines[..index];
                    while (condition(lines.Span[index + 1]))
                    {
                        index++;
                    }
                    rest = includeLine
                        ? lines[index..]
                        : lines[(index + 1)..];

                    return content;
                }
            }

            rest = ReadOnlyMemory<ReadOnlyMemory<char>>.Empty;
            return lines;
        }

        public static ReadOnlyMemory<ReadOnlyMemory<char>> SliceUntilBlankLine(ReadOnlyMemory<ReadOnlyMemory<char>> lines, out ReadOnlyMemory<ReadOnlyMemory<char>> rest)
            => SliceUntilCondition(lines, static span => span.IsEmpty, false, out rest);

        public static IEnumerable<ReadOnlyMemory<ReadOnlyMemory<char>>> SplitByBlankLines(ReadOnlyMemory<ReadOnlyMemory<char>> lines)
        {
            while (lines.IsEmpty is false)
            {
                var slice = SliceUntilBlankLine(lines, out lines);
                yield return slice;
            }
        }

        public static IEnumerable<ReadOnlyMemory<ReadOnlyMemory<char>>> SplitByCondition(
            ReadOnlyMemory<ReadOnlyMemory<char>> lines,
            Func<ReadOnlyMemory<char>, bool> condition,
            bool includeLine)
        {
            while (lines.IsEmpty is false)
            {
                var slice = SliceUntilCondition(lines, condition, includeLine, out lines);
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
                    var number = span[..index];
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

        public static void ConcatInto(
            this Span<char> span,
            ReadOnlySpan<char> span0,
            ReadOnlySpan<char> span1)
        {
            Debug.Assert(span.Length == span0.Length + span1.Length);

            span0.CopyTo(span);
            span1.CopyTo(span[span0.Length..]);
        }

        public static bool SequenceEqual<T>(this Span2D<T> a, Span2D<T> b) where T : IEquatable<T>
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (var x = 0; x < a.Width; x++)
            {
                var aRow = a.GetRowSpan(x);
                var bRow = b.GetRowSpan(x);

                if (aRow.SequenceEqual(bRow) is false)
                {
                    return false;
                }
            }

            return true;
        }



        public static ReadOnlySpan<T> RemoveSequence<T>(
            this ReadOnlySpan<T> span,
            ReadOnlySpan<T> find)
                where T : IEquatable<T>
        {
            var ranges = new List<Range>();

            var count = 0;

            var start = 0;
            var index = span.IndexOf(find);
            while (index != -1)
            {
                count++;

                if (start > 0)
                {
                    var range = start..index;
                    ranges.Add(range);
                    start = index + find.Length;
                }
                else
                {
                    start = index + find.Length;
                }

                index = span[start..].IndexOf(find);

                if (index is not -1)
                {
                    index = start + span[start..].IndexOf(find);
                }
            }

            if (start != span.Length)
            {
                ranges.Add(start..span.Length);
            }

            var position = 0;
            Span<T> next = new T[span.Length - (find.Length * count)];
            foreach (var range in ranges)
            {
                var section = span[range];
                section.CopyTo(next[position..]);
                position += section.Length;
            }

            return next;
        }

        public static ReadOnlySpan<T> ReplaceSequence<T>(
            this ReadOnlySpan<T> span,
            ReadOnlySpan<T> find,
            ReadOnlySpan<T> replacement)
                where T : IEquatable<T>
        {
            var ranges = new List<Range>();

            var count = 0;
            var index = span.IndexOf(find);
            while (index != -1)
            {
                ranges.Add(index..(index + find.Length));
                count++;

                var start = index + find.Length;
                index = span[start..].IndexOf(find);

                if (index is not -1)
                {
                    index = start + span[start..].IndexOf(find);
                }
            }

            var position = 0;
            var nextPosition = 0;
            Span<T> next = new T[span.Length - (find.Length * count) + (replacement.Length * count)];
            for (var i = 0; i < ranges.Count; i++)
            {
                var range = ranges[i];

                if (position == range.Start.Value)
                {
                    replacement.CopyTo(next[nextPosition..]);
                    nextPosition += replacement.Length;
                    position = range.End.Value;
                }
                else
                {
                    var section = span[position..range.Start.Value];
                    section.CopyTo(next[nextPosition..]);
                    nextPosition += section.Length;
                    position += section.Length;
                    i--;
                }
            }

            if (position != span.Length)
            {
                var section = span[position..];
                section.CopyTo(next[nextPosition..]);
            }

            return next;
        }
    }
}
