using Shared.Grid;

namespace Shared
{
    public abstract class GridScanner
    {
        private readonly Area2d _dimensions;

        public GridScanner(Area2d dimensions)
        {
            _dimensions = dimensions;
        }

        public void Scan()
        {
            foreach (var point in _dimensions.Items)
            {

            }

            var (xRange, yRange) = _dimensions;

            for (var y = yRange.Min; y <= yRange.Max; y++)
            {
                for (var x = xRange.Min; x <= xRange.Max; x++)
                {
                    var point = new Point2d(x, y);
                    OnPosition(point);
                }
                OnEndOfRow();
            }
        }

        public abstract void OnPosition(Point2d point);

        public abstract void OnEndOfRow();
    }
}
