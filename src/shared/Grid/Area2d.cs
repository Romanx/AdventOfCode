using System;
using System.Collections.Generic;

namespace Shared.Grid
{
    public class Area2d
    {
        private readonly DimensionRange _xRange;
        private readonly DimensionRange _yRange;

        public Area2d(DimensionRange xRange, DimensionRange yRange)
        {
            _xRange = xRange;
            _yRange = yRange;
            Height = _yRange.Max - yRange.Min + 1;
            Width = _xRange.Max - xRange.Min + 1;

            Middle = new Point2d(
                (_xRange.Min + _xRange.Max) / 2,
                (_yRange.Min + _yRange.Max) / 2);

            TopLeft = new Point2d(_xRange.Min, _yRange.Max);
            TopRight = new Point2d(_xRange.Max, _yRange.Max);
            BottomLeft = new Point2d(_xRange.Min, _yRange.Min);
            BottomRight = new Point2d(_xRange.Max, _yRange.Min);

            Count = _xRange.Count * _yRange.Count;
        }

        public bool Contains(Point2d point)
        {
            if (point.X < _xRange.Min || point.X > _xRange.Max || point.Y < _yRange.Min || point.Y > _yRange.Max)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<Point2d> Items => GetItems();

        public int Count { get; }
        public Point2d TopLeft { get; }
        public Point2d TopRight { get; }
        public Point2d BottomLeft { get; }
        public Point2d BottomRight { get; }
        public Point2d Middle { get; }

        public int Height { get; }

        public int Width { get; }

        public void Deconstruct(out DimensionRange x, out DimensionRange y)
            => (x, y) = (_xRange, _yRange);

        public override string ToString()
        {
            return $"[{_xRange.Min},{_yRange.Min}] -> [{_xRange.Max},{_yRange.Max}]";
        }

        private IEnumerable<Point2d> GetItems()
        {
            for (var y = _yRange.Min; y <= _yRange.Max; y++)
            {
                for (var x = _xRange.Min; x <= _xRange.Max; x++)
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
            var xMax = array.GetLength(0) - 1;
            var yMax = array.GetLength(1) - 1;

            var xRange = new DimensionRange(0, xMax);
            var yRange = new DimensionRange(0, yMax);

            return new Area2d(xRange, yRange);
        }
    }
}
