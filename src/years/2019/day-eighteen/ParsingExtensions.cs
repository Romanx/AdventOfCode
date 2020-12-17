using System;
using System.Collections.Immutable;
using Shared;

namespace DayEighteen2019
{
    internal static class ParsingExtensions
    {
        internal static Map Parse(this IInput input)
        {
            var map = ImmutableDictionary.CreateBuilder<Point2d, Cell>();
            var array = input.As2DArray();

            for (var x = 0; x < array.GetLength(0); x++)
            {
                for (var y = 0; y < array.GetLength(1); y++)
                {
                    Point2d point = new(x, y);

                    map[point] = array[x, y] switch
                    {
                        '@' => new Cell(CellType.Entrance, point),
                        '#' => new Cell(CellType.Wall, point),
                        '.' => new Cell(CellType.Empty, point),
                        >= 'a' and <= 'z' => new Key(array[x, y], point),
                        >= 'A' and <= 'Z' => new Door(array[x, y], char.ToLowerInvariant(array[x, y]), point),
                        _ => throw new InvalidOperationException()
                    };
                }
            }

            return new Map(map.ToImmutable());
        }
    }
}
