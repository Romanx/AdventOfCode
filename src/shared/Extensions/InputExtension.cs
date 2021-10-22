using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Toolkit.HighPerformance;
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
            var array = new char[lines[0].Length, lines.Length];

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y].AsSpan();
                for (var x = 0; x < line.Length; x++)
                {
                    array[x, y] = line[x];
                }
            }

            return array;
        }

        public static IEnumerable<int> CharactersToInt(this IInputContent content)
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

        public static ReadOnlyMemory<ReadOnlyMemory<char>>[] AsParagraphs(this IInputLines lines)
        {
            var body = lines.AsMemory().ToArray().AsMemory();

            return SpanHelpers.SplitByBlankLines(body).ToArray();
        }

        public static ImmutableArray<(Point2d Point, char Character)> As2DPoints(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<(Point2d Point, char C)>();
            var arr = input.Lines.As2DArray().AsSpan2D();

            for (var row = 0; row < arr.Height; row++)
            {
                for (var column = 0; column < arr.Width; column++)
                {
                    builder.Add((new Point2d(row, column), arr[row, column]));
                }
            }

            return builder.ToImmutable();
        }
    }
}
