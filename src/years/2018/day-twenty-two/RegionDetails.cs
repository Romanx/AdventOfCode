using System;
using System.Collections.Generic;
using Shared;

namespace DayTwentyTwo2018
{
    record RegionDetails(CellType CellType, ulong GeologicIndex, ulong ErosionLevel)
    {
        public static RegionDetails Calculate(
            Point2d start,
            Point2d target,
            Point2d current,
            ulong depth,
            IDictionary<Point2d, RegionDetails> map)
        {
            if (map.TryGetValue(current, out var cached))
            {
                return cached;
            }

            var geologicIndex = GeologicIndex(start, target, current, map);
            var erosionLevel = ErosionLevel(geologicIndex, depth);
            var ct = CalculateCellType(erosionLevel);

            var result = new RegionDetails(ct, geologicIndex, erosionLevel);
            map[current] = result;
            return result;

            static CellType CalculateCellType(ulong erosionLevel)
            {
                return (erosionLevel % 3) switch
                {
                    0 => CellType.Rocky,
                    1 => CellType.Wet,
                    2 => CellType.Narrow,
                    _ => throw new InvalidOperationException("Can't work out cell type"),
                };
            }

            ulong GeologicIndex(Point2d start, Point2d target, Point2d current, IDictionary<Point2d, RegionDetails> map)
            {
                return current switch
                {
                    _ when current == target => 0,
                    _ when current == start => 0,
                    { Y: 0 } => (ulong)current.X * 16807L,
                    { X: 0 } => (ulong)current.Y * 48271L,
                    _ => NeighbourBased(current, map),
                };

                ulong NeighbourBased(Point2d current, IDictionary<Point2d, RegionDetails> map)
                {
                    var leftPoint = current + GridDirection.Left;
                    var left = map.TryGetValue(leftPoint, out var leftDetails)
                        ? leftDetails
                        : Calculate(target, start, leftPoint, depth, map);

                    var upPoint = current + GridDirection.Up;
                    var up = map.TryGetValue(upPoint, out var upDetails)
                        ? upDetails
                        : Calculate(target, start, upPoint, depth, map);

                    return left.ErosionLevel * up.ErosionLevel;
                }
            }

            static ulong ErosionLevel(ulong geologicIndex, ulong depth) => (geologicIndex + depth) % 20183;
        }
    }
}
