﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Shared
{
    public static class InputExtension
    {
        public static IEnumerable<int> FromIntString(this IInput input) => input
            .AsString()
            .ToCharArray()
            .Select(c => int.Parse(c.ToString()));

        public static ImmutableArray<(Point2d point, char)> As2DPoints(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<(Point2d Point, char C)>();
            var arr = input.As2DArray().AsSpan2D();

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
