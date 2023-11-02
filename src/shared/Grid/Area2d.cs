using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using CommunityToolkit.HighPerformance;

namespace Shared.Grid
{
    public readonly record struct Area2d
    {
        public Area2d(DimensionRange xRange, DimensionRange yRange)
        {
            XRange = xRange;
            YRange = yRange;
            Height = YRange.Max - yRange.Min + 1;
            Width = XRange.Max - xRange.Min + 1;

            Middle = new Point2d(
                (XRange.Min + XRange.Max) / 2,
                (YRange.Min + YRange.Max) / 2);

            TopLeft = new Point2d(XRange.Min, YRange.Max);
            TopRight = new Point2d(XRange.Max, YRange.Max);
            BottomLeft = new Point2d(XRange.Min, YRange.Min);
            BottomRight = new Point2d(XRange.Max, YRange.Min);

            Count = (long)XRange.Size * (long)YRange.Size;
        }

        public bool Contains(Point2d point)
        {
            if (point.X < XRange.Min || point.X > XRange.Max || point.Y < YRange.Min || point.Y > YRange.Max)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<Point2d> Items => GetItems();
        public DimensionRange XRange { get; }
        public DimensionRange YRange { get; }
        public long Count { get; }
        public Point2d TopLeft { get; }
        public Point2d TopRight { get; }
        public Point2d BottomLeft { get; }
        public Point2d BottomRight { get; }
        public Point2d Middle { get; }

        public int Height { get; }

        public int Width { get; }

        public void Deconstruct(out DimensionRange x, out DimensionRange y)
            => (x, y) = (XRange, YRange);

        public override string ToString()
        {
            return $"[{XRange.Min},{YRange.Min}] -> [{XRange.Max},{YRange.Max}]";
        }

        private IEnumerable<Point2d> GetItems()
        {
            for (var y = YRange.Min; y <= YRange.Max; y++)
            {
                for (var x = XRange.Min; x <= XRange.Max; x++)
                {
                    yield return new Point2d(x, y);
                }
            }
        }

        public bool Intersects(Area2d other)
            => XRange.Intersects(other.XRange) && YRange.Intersects(other.YRange);

        public Area2d? Intersect(Area2d other)
        {
            if (Intersects(other) is false)
            {
                return null;
            }

            return new Area2d(
                XRange.Intersect(other.XRange),
                YRange.Intersect(other.YRange));
        }

        public Area2d Add(IEnumerable<Point2d> points)
        {
            var minX = XRange.Min;
            var maxX = XRange.Max;
            var minY = YRange.Min;
            var maxY = YRange.Max;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                minY = Math.Min(minY, point.Y);
                maxY = Math.Max(maxY, point.Y);
            }

            return new Area2d(
                new DimensionRange(minX, maxX),
                new DimensionRange(minY, maxY));
        }

        public static Area2d Create(string start, string end)
            => Create(Point2d.Parse(start), Point2d.Parse(end));

        public static Area2d Create(Point2d start, Point2d end)
        {
            var xRange = new DimensionRange(
                Math.Min(start.X, end.X),
                Math.Max(start.X, end.X));

            var yRange = new DimensionRange(
                Math.Min(start.Y, end.Y),
                Math.Max(start.Y, end.Y));

            return new Area2d(xRange, yRange);
        }

        public static Area2d Create(IEnumerable<Point2d> points)
        {
            var dimensions = PointHelpers.FindSpaceOfPoints(points);

            return new Area2d(
                dimensions[0],
                dimensions[1]
            );
        }

        public static Area2d Create<T>(T[,] array)
        {
            var yMax = array.GetLength(0) - 1;
            var xMax = array.GetLength(1) - 1;

            var xRange = new DimensionRange(0, xMax);
            var yRange = new DimensionRange(0, yMax);

            return new Area2d(xRange, yRange);
        }

        public static Area2d Create(int width, int height)
        {
            var xRange = new DimensionRange(0, width - 1);
            var yRange = new DimensionRange(0, height - 1);

            return new Area2d(xRange, yRange);
        }

        public static Area2d Create<T>(ReadOnlySpan2D<T> span)
        {
            var xRange = new DimensionRange(0, span.Width - 1);
            var yRange = new DimensionRange(0, span.Height - 1);

            return new Area2d(xRange, yRange);
        }

        public static Builder CreateBuilder() => new();

        public sealed class Builder
        {
            private bool initialized = true;
            private int minX;
            private int maxX;

            private int minY;
            private int maxY;

            public void Add(Point2d point)
            {
                if (initialized is false)
                {
                    minX = point.X;
                    maxX = point.X;
                    minY = point.Y;
                    maxY = point.Y;
                    initialized = true;
                    return;
                }

                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                minY = Math.Min(minY, point.Y);
                maxY = Math.Max(maxY, point.Y);
            }

            public void AddRange(IEnumerable<Point2d> points)
            {
                foreach (var point in points)
                {
                    Add(point);
                }
            }

            public Area2d Build()
            {
                if (initialized is false)
                {
                    throw new InvalidOperationException("Unable to build an area with no points.");
                }

                return new(
                    new DimensionRange(minX, maxX),
                    new DimensionRange(minY, maxY));
            }
        }
    }
}
