using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.HighPerformance;
using Shared.Helpers;

namespace Shared
{
    public static class InputExtension
    {
        public static string[] AsArray(this IInputLines lines)
            => lines.AsString().ToArray();

        public static ReadOnlySpan<char> AsSpan(this IInputContent content)
            => content.AsString().AsSpan();

        public static int AsInt(this IInputContent content)
            => int.Parse(content.AsString());

        public static char[,] As2DArray(this IInputLines inputLines)
        {
            var lines = inputLines.AsArray();
            var array = new char[lines.Length, lines[0].Length];

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y].AsSpan();
                for (var x = 0; x < line.Length; x++)
                {
                    array[y, x] = line[x];
                }
            }

            return array;
        }

        public static T[,] As2DArray<T>(
            this IInputLines inputLines,
            Func<char, T> converter)
        {
            var lines = inputLines.AsArray();
            var array = new T[lines.Length, lines[0].Length];

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y].AsSpan();
                for (var x = 0; x < line.Length; x++)
                {
                    array[y, x] = converter(line[x]);
                }
            }

            return array;
        }

        public static int[] CharactersToInt(this IInputContent content)
        {
            var span = content.AsSpan();
            var result = new int[span.Length];
            for (var i = 0; i < result.Length; i++)
            {
                if (char.IsDigit(span[i]) is false)
                {
                    throw new InvalidOperationException($"Character '{span[i]}' is not a digit");
                }

                result[i] = span[i] - '0';
            }

            return result;
        }

        public static IEnumerable<int> Ints(this IInputLines lines)
            => lines.Transform(static l => int.Parse(l));

        public static IEnumerable<int> AsInts(this IInputContent content, char separator = ',')
            => content.AsString()
                .Split(separator)
                .Select(i => int.Parse(i));

        public static ReadOnlyMemory<ReadOnlyMemory<char>>[] AsParagraphs(this IInputLines lines)
        {
            var body = lines.AsMemory().ToArray().AsMemory();

            return SpanHelpers.SplitByBlankLines(body).ToArray();
        }

        public static IEnumerable<(Point2d Point, char Character)> As2DPoints(this IInput input)
        {
            var arr = input.Lines.As2DArray().AsMemory2D();

            for (var y = 0; y < arr.Span.Height; y++)
            {
                for (var x = 0; x < arr.Span.Width; x++)
                {
                    yield return (new Point2d(x, y), arr.Span[y, x]);
                }
            }
        }

        public static IEnumerable<T> As<T>(this IInputLines lines)
            where T : ISpanParsable<T>
        {
            return lines.Transform(static str =>
            {
                return T.Parse(str, null);
            });
        }

        public static IEnumerable<T> AsParsable<T>(this IInputLines lines)
            where T : IParsable<T>
        {
            return lines.Transform(static str =>
            {
                return T.Parse(str, null);
            });
        }
    }
}
