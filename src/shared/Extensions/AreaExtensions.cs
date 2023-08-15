using Shared.Grid;

namespace Shared
{
    public static class AreaExtensions
    {
        public static T[,] As2dArray<T>(this Area2d area)
        {
            return new T[area.Height, area.Width];
        }

        public static Area2d Pad(this Area2d area, int value)
        {
            var (xRange, yRange) = area;

            return new Area2d(
                xRange.Pad(value),
                yRange.Pad(value));
        }

        public static Area3d Pad(this Area3d area, int value)
        {
            var (xRange, yRange, zRange) = area;

            return new Area3d(
                xRange.Pad(value),
                yRange.Pad(value),
                zRange.Pad(value));
        }
    }
}
