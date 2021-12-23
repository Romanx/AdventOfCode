using Shared.Grid;

namespace Shared
{
    public static class AreaExtensions
    {
        public static T[,] As2dArray<T>(this Area2d area)
        {
            return new T[area.BottomRight.Y + 1, area.BottomRight.X + 1];
        }

        public static Area2d Pad(this Area2d area, int value)
        {
            var (xRange, yRange) = area;

            return new Area2d(
                xRange.Pad(value),
                yRange.Pad(value));
        }
    }
}
