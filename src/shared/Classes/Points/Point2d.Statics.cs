using System;

namespace Shared
{
    public readonly partial record struct Point2d
    {
        public static int ManhattanDistance(Point2d left, Point2d right)
            => PointHelpers.ManhattanDistance(left, right);

        public static int Distance(Point2d left, Point2d right)
        {
            return (int)Math.Sqrt(Math.Pow(right.X - left.X, 2) + Math.Pow(right.Y - left.Y, 2));
        }

        public static Direction DirectionBetweenPoints(Point2d left, Point2d right)
        {
            var angle = PointHelpers.AngleInDegrees(left, right);

            if (angle is 0)
            {
                return Direction.North;
            }
            else if (angle > 0 && angle < 90)
            {
                return Direction.NorthEast;
            }
            else if (angle is 90)
            {
                return Direction.East;
            }
            else if (angle > 90 && angle < 180)
            {
                return Direction.SouthEast;
            }
            else if (angle is 180)
            {
                return Direction.South;
            }
            else if (angle > 180 && angle < 270)
            {
                return Direction.SouthWest;
            }
            else if (angle is 270)
            {
                return Direction.West;
            }
            else if (angle > 270 && angle < 360)
            {
                return Direction.NorthWest;
            }

            throw new InvalidOperationException("Unable to calculate direction between points");
        }
    }
}
