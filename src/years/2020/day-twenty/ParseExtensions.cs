using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Shared;
using Shared.Helpers;

namespace DayTwenty2020
{
    internal static class ParseExtensions
    {
        public static ImmutableArray<Tile> ParseTiles(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Tile>();

            foreach (var block in SplitByBlankLines(input))
            {
                builder.Add(ParseBlock(block));
            }

            return builder.ToImmutable();

            static Tile ParseBlock(Memory<ReadOnlyMemory<char>> block)
            {
                var firstLine = block.Span[0].Span;

                var number = int.Parse(firstLine[(firstLine.IndexOf(' ') + 1)..^1]);

                var picture = SpanHelpers.As2dArray(block[1..]);

                return new Tile(number, picture);
            }
        }
    }
}
