namespace Shared.Grid
{
    public class Area3d
    {
        private readonly DimensionRange _xRange;
        private readonly DimensionRange _yRange;
        private readonly DimensionRange _zRange;

        public Area3d(DimensionRange xRange, DimensionRange yRange, DimensionRange zRange)
        {
            _xRange = xRange;
            _yRange = yRange;
            _zRange = zRange;
        }

        public bool Contains(Point3d point)
        {
            return point.X >= _xRange.Min &&
                   point.X <= _xRange.Max &&
                   point.Y >= _yRange.Min &&
                   point.Y <= _yRange.Max &&
                   point.Z >= _zRange.Min &&
                   point.Z <= _zRange.Max;
        }

        public override string ToString()
        {
            return $"[{_xRange.Min},{_yRange.Min},{_zRange.Min}] -> [{_xRange.Max},{_yRange.Max},{_zRange.Max}]";
        }
    }
}
