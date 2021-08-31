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

        public bool Contains(Point3d point)
        {
            return point.X >= _xRange.Min &&
                   point.X <= _xRange.Max &&
                   point.Y >= _yRange.Min &&
                   point.Y <= _yRange.Max;
        }

        public int Count => _xRange.Count * _yRange.Count;

        public IEnumerable<Point2d> Items => GetItems();

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
    }
}
