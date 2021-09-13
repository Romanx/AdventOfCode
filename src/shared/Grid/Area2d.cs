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
        }

        public bool Contains(Point2d point)
        {
            return point.X >= _xRange.Min &&
                   point.X <= _xRange.Max &&
                   point.Y >= _yRange.Min &&
                   point.Y <= _yRange.Max;
        }

        public int Count => _xRange.Count * _yRange.Count;

        public IEnumerable<Point2d> Items => GetItems();

        public Point2d TopLeft => new(_xRange.Min, _yRange.Min);
        public Point2d TopRight => new(_xRange.Max, _yRange.Min);
        public Point2d BottomLeft => new(_xRange.Min, _yRange.Max);
        public Point2d BottomRight => new(_xRange.Max, _yRange.Max);

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
    }
}
