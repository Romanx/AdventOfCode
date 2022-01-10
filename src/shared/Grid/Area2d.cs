﻿using System;
using System.Collections.Generic;

namespace Shared.Grid
{
    public class Area2d
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

            Count = XRange.Size * YRange.Size;
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
        public int Count { get; }
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
            var (x, y) = Point2d.FindSpaceOfPoints(points);

            return new Area2d(
                new DimensionRange(x.Min, x.Max),
                new DimensionRange(y.Min, y.Max)
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
    }
}
