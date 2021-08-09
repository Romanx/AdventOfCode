using System.Collections.Immutable;
using Shared;

namespace DayTwentyTwo2018
{
    internal static class MapGenerator
    {
        public static ImmutableDictionary<Point2d, RegionDetails> Generate(Point2d start, Point2d target, ulong depth)
        {
            var map = ImmutableDictionary.CreateBuilder<Point2d, RegionDetails>();

            var xRange = new GridRange(start.X, target.X);
            var yRange = new GridRange(start.Y, target.Y);

            for (var y = yRange.Min; y <= yRange.Max; y++)
            {
                for (var x = xRange.Min; x <= xRange.Max; x++)
                {
                    var point = new Point2d(x, y);
                    map[point] = RegionDetails.Calculate(start, target, point, depth, map);
                }
            }

            return map.ToImmutable();
        }
    }
}
