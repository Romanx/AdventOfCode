﻿using System;

namespace Shared
{
    public class GridDimensions
    {
        private readonly GridRange _x;
        private readonly GridRange _y;

        public GridDimensions(GridRange x, GridRange y)
        {
            _x = x;
            _y = y;
        }

        public bool Contains(Point2d point)
        {
            return point.X >= _x.Min &&
                   point.X <= _x.Max &&
                   point.Y >= _y.Min &&
                   point.Y <= _y.Max;
        }

        public void Deconstruct(out GridRange x, out GridRange y) => (x, y) = (_x, _y);
    }
}
