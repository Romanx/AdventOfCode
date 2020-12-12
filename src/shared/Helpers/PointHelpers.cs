using System;

namespace Shared.Helpers
{
    public static class PointHelpers
    {
        public static int CrossProduct(Point a, Point b)
        {
            return (a.X * b.Y) - (b.X * a.Y);
        }

        public static double AngleInRadians(Point a, Point b)
            => Math.Atan2(b.X - a.X, -(b.Y - a.Y));

        public static double AngleInDegrees(Point a, Point b)
        {
            var rad = AngleInRadians(a, b);

            return (rad >= 0 ? rad : (2 * Math.PI + rad)) * 360 / (2 * Math.PI);
        }

        public static int ManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
