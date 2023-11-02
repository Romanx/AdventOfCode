using System;
using System.Diagnostics;

namespace Shared
{
    public readonly partial record struct Point2d
    {
        /// <summary>
        /// The manhanttan distance between two points
        /// http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#manhattan-distance
        /// </summary>
        /// <param name="left">The left point</param>
        /// <param name="right">The right point</param>
        /// <returns>The distance between the two points only cardinal movement</returns>
        public static int ManhattanDistance(Point2d left, Point2d right)
            => PointHelpers.ManhattanDistance(left, right);

        /// <summary>
        /// The diagonal distance between two points
        /// http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#diagonal-distance
        /// </summary>
        /// <param name="left">The left point</param>
        /// <param name="right">The right point</param>
        /// <returns>The distance between the two points including diagonal movement</returns>
        public static int DiagonalDistance(Point2d left, Point2d right)
        {
            var dx = int.Abs(left.X - right.X);
            var dy = int.Abs(left.Y - right.Y);

            return dx + dy - int.Min(dx, dy);
        }

        /// <summary>
        /// Calculate the direction between the two points
        /// </summary>
        /// <param name="left">The left point</param>
        /// <param name="right">The right point</param>
        /// <returns>The direction between the two different points</returns>
        public static Direction DirectionBetweenPoints(Point2d left, Point2d right)
        {
            var xDiff = right.X - left.X;
            var yDiff = right.Y - left.Y;

            return (xDiff, yDiff) switch
            {
                (< 0,   0) => Direction.West,
                (> 0,   0) => Direction.East,
                (  0, < 0) => Direction.North,
                (  0, > 0) => Direction.South,

                (> 0, > 0) => Direction.SouthEast,
                (> 0, < 0) => Direction.NorthEast,

                (< 0, < 0) => Direction.NorthWest,
                (< 0, > 0) => Direction.SouthWest,

                (  0,   0) => Direction.None,
            };
        }

        public static int SlopeBetweenTwoPoints(Point2d a, Point2d b)
        {
            return (b.Y - a.Y) / (b.X - a.X);
        }
    }
}
