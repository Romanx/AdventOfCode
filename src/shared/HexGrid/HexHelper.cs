using System;

namespace Shared.HexGrid
{
    public static class HexHelper
    {
        public static int ManhattanDistance(Hex start, Hex target)
        {
            var a = start.Point;
            var b = target.Point;

            return (Math.Abs(a.X - b.X) +
                   Math.Abs(a.Y - b.Y) +
                   Math.Abs(a.Z - b.Z)) / 2;
        }
    }
}
