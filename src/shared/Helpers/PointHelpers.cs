﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using MoreLinq;

namespace Shared.Helpers
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

        public static int ManhattanDistance(Point2d a, Point2d b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public static IEnumerable<T> GetNeighboursInDistance<T>(this T point, int distance, Func<IEnumerable<int>, T> factory)
            where T : Point
        {
            var combinations = point.Dimensions
                .Select(d => MoreEnumerable.Sequence(d - distance, d + distance))
                .CartesianProduct();

            foreach (var combin in combinations)
            {
                var pt = factory(combin);
                if (pt != point)
                {
                    yield return pt;
                }
            }
        }

        public static ImmutableArray<GridRange> FindSpaceOfPoints<T>(IEnumerable<T> points, int dimensions)
            where T : Point
        {
            var dimensionRanges = Enumerable.Range(0, dimensions)
                .Select(_ => (Min: 0, Max: 0))
                .ToArray();

            foreach (var point in points)
            {
                for (var i = 0; i < point.Dimensions.Length; i++)
                {
                    var dimensionVal = point.Dimensions[i];
                    var dimensionRange = dimensionRanges[i];

                    if (dimensionVal < dimensionRange.Min)
                    {
                        dimensionRange.Min = dimensionVal;
                    }
                    else if (dimensionVal > dimensionRange.Max)
                    {
                        dimensionRange.Max = dimensionVal;
                    }
                    dimensionRanges[i] = dimensionRange;
                }
            }

            return dimensionRanges
                .Select(r => new GridRange(r.Min, r.Max))
                .ToImmutableArray();
        }

        public static IEnumerable<T> PointsInSpace<T>(IEnumerable<GridRange> ranges, Func<IEnumerable<int>, T> factory)
            where T : Point
        {
            var cart = ranges.CartesianProduct();

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
    }
}
