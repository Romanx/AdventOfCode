using System.Collections.Immutable;
using Shared;
using Shared.Grid;

namespace DayTwentyTwo2018
{
    internal static class MapGenerator
    {
        public static ImmutableDictionary<Point2d, RegionDetails> Generate(Point2d start, Point2d target, ulong depth)
        {
            var map = ImmutableDictionary.CreateBuilder<Point2d, RegionDetails>();

            var area = Area2d.Create(start, target);

            foreach (var point in area.Items)
            {
                map[point] = RegionDetails.Calculate(start, target, point, depth, map);
            }

            return map.ToImmutable();
        }
    }
}
