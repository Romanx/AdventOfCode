using System;

namespace Shared
{
    public record Point(int Row, int Column) : IComparable<Point?>
    {
        public int X => Row;

        public int Y => Column;

        public static Point Origin { get; } = new(0, 0);

        public int CompareTo(Point? other) =>
            other is null
                ? -1
                : (Row, Column).CompareTo((other.Row, other.Column));

        public static Point operator +(Point left, Point right)
            => new(left.Row + right.Row, left.Column + right.Column);

        public static Point operator -(Point left, Point right)
            => new(left.Row - right.Row, left.Column - right.Column);

        public static Point operator +(Point point, Direction direction) => direction.DirectionType switch
        {
            DirectionType.North => point + (0, -1),
            DirectionType.NorthEast => point + (1, -1),
            DirectionType.NorthWest => point + (-1, -1),
            DirectionType.East => point + (1, 0),
            DirectionType.South => point + (0, 1),
            DirectionType.SouthEast => point + (1, 1),
            DirectionType.SouthWest => point + (-1, 1),
            DirectionType.West => point + (-1, 0),
            _ => point
        };

        public static Point AddInDirection(Point point, Direction direction, int count)
        {
            return direction.DirectionType switch
            {
                DirectionType.North => point + (0, -count),
                DirectionType.NorthEast => point + (count, -count),
                DirectionType.NorthWest => point + (-count, -count),
                DirectionType.East => point + (count, 0),
                DirectionType.South => point + (0, count),
                DirectionType.SouthEast => point + (count, count),
                DirectionType.SouthWest => point + (-count, count),
                DirectionType.West => point + (-count, 0),
                _ => point
            };
        }

        public Point RotateAroundPivot(Point pivot, int angleInDegrees)
        {
            var radians = angleInDegrees * MathF.PI / 180;

            var s = MathF.Sin(radians);
            var c = MathF.Cos(radians);

            var translated = this - pivot;

            // Rotate
            var xnew = (int)MathF.Round(translated.X * c - translated.Y * s);
            var ynew = (int)MathF.Round(translated.X * s + translated.Y * c);

            return new Point(xnew + pivot.X, ynew + pivot.Y);
        }

        public static implicit operator Point((int X, int Y) i) => new(i.X, i.Y);
    }
}
