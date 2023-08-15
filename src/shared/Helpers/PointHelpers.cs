using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using MoreLinq;
using Shared.Grid;
using Shared.Helpers;

namespace Shared
{
    public static class PointHelpers
    {
        public static int CrossProduct(Point2d a, Point2d b)
        {
            return (a.X * b.Y) - (b.X * a.Y);
        }

        public static double AngleInRadians(Point2d a, Point2d b)
            => Math.Atan2(b.X - a.X, -(b.Y - a.Y));

        public static double AngleInDegrees(Point2d a, Point2d b)
        {
            var rad = AngleInRadians(a, b);

            return (rad >= 0 ? rad : (2 * Math.PI + rad)) * 360 / (2 * Math.PI);
        }

        public static int ManhattanDistance<T>(T a, T b)
            where T : IPoint
        {
            var result = 0;
            Span<int> aDimensions = stackalloc int[T.DimensionCount];
            Span<int> bDimensions = stackalloc int[T.DimensionCount];
            a.GetDimensions(aDimensions);
            b.GetDimensions(bDimensions);

            for (var i = 0; i < T.DimensionCount; i++)
            {
                result += int.Abs(aDimensions[i] - bDimensions[i]);
            }

            return result;
        }

        public static IEnumerable<T> GetNeighboursInDistance<T>(this T point, int distance, Func<IEnumerable<int>, T> factory)
            where T : IPoint, IEquatable<T>
        {
            var scratch = new int[T.DimensionCount];
            point.GetDimensions(scratch);

            var combinations = scratch
                .Select(d => MoreEnumerable.Sequence(d - distance, d + distance))
                .CartesianProduct();

            foreach (var combin in combinations)
            {
                var pt = factory(combin);
                if (!pt.Equals(point))
                {
                    yield return pt;
                }
            }
        }

        public static ImmutableArray<DimensionRange> FindSpaceOfPoints<T>(IEnumerable<T> points)
            where T : IPoint
        {
            var dimensionCount = T.DimensionCount;

            var ranges = new (int? Min, int? Max)[dimensionCount];
            Span<int> scratch = stackalloc int[dimensionCount]; 

            foreach (var point in points)
            {
                point.GetDimensions(scratch);

                for (var i = 0; i < dimensionCount; i++)
                {
                    var val = scratch[i];
                    var (min, max) = ranges[i];

                    var nextMin = min is null || val < min
                        ? val
                        : min;

                    var nextMax = max is null || val > max
                        ? val
                        : max;

                    ranges[i] = (nextMin, nextMax);
                }
            }

            var builder = ImmutableArray.CreateBuilder<DimensionRange>(dimensionCount);
            for (var i = 0; i < dimensionCount; i++)
            {
                var (min, max) = ranges[i];

                builder.Add(new DimensionRange(min!.Value, max!.Value));
            }

            return builder.MoveToImmutable();
        }

        public static IEnumerable<Point2d> GetDirectNeighbours(Point2d point)
            => Directions.CardinalDirections.Select(dir => point + dir);

        public static IEnumerable<Point2d> GetNeighbours(Point2d point)
            => Directions.All.Select(dir => point + dir);

        public static AdjacentPoints2d GetNeighbours(
            Point2d point,
            ISet<Point2d> points,
            AdjacencyType adjacencyType)
            => new(point, points, adjacencyType);

        public static AdjacentPoints2d GetNeighbours(
            Point2d point,
            AdjacencyType adjacencyType)
            => new(point, null, adjacencyType);

        public static IEnumerable<T> PointsInSpace<T>(IEnumerable<DimensionRange> ranges, Func<IEnumerable<int>, T> factory)
            where T : IPoint
        {
            var cart = ranges
                .Select(range => MoreEnumerable.Sequence(range.Min, range.Max))
                .CartesianProduct();

            return cart.Select(num => factory(num));
        }

        public static ImmutableDictionary<Point2d, char> StringToPoints(string str)
        {
            var builder = ImmutableDictionary.CreateBuilder<Point2d, char>();
            using var reader = new StringReader(str);

            string? line;
            var rowNum = 0;
            while ((line = reader.ReadLine()) != null)
            {
                builder.AddRange(LineToPoints(rowNum, line));
                rowNum++;
            }

            return builder.ToImmutable();

            IEnumerable<KeyValuePair<Point2d, char>> LineToPoints(int rowNum, string str) => str
                .ToCharArray()
                .Index()
                .Select(kvp => KeyValuePair.Create(new Point2d(rowNum, kvp.Key), kvp.Value));
        }

        public static IEnumerable<AdjacentPoints2d> Intersections(ImmutableHashSet<Point2d> points)
        {
            foreach (var point in points)
            {
                var adjacent = new AdjacentPoints2d(point, points, AdjacencyType.Cardinal);
                if (adjacent.Count is 4)
                    yield return adjacent;
            }
        }
    }
}
