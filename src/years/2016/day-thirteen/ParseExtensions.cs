using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Shared;
using Shared.Grid;
using Spectre.Console;

namespace DayThirteen2016
{
    static class ParseExtensions
    {
        public static OfficeSpace Parse(this IInput input)
        {
            var favouriteNumber = int.Parse(input.Content.AsString());
            var builder = ImmutableDictionary.CreateBuilder<Point2d, CellType>();

            var area2d = Area2d.Create(Point2d.Origin, new Point2d(51, 51));

            var items = area2d.Items.Select(point => KeyValuePair.Create(
                point, IsWall(point) ? CellType.Wall : CellType.Empty));

            builder.AddRange(items);

            return new OfficeSpace(builder.ToImmutable());

            bool IsWall(Point2d point)
            {
                var value = point.X * point.X + 3 * point.X + 2 * point.X * point.Y + point.Y + point.Y * point.Y;
                value += favouriteNumber;
                var bitsSet = BitOperations.PopCount((uint)value);

                return bitsSet % 2 != 0;
            }
        }
    }
}
