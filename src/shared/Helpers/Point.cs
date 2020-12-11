using System;

namespace Shared
{
    public record Point(int Row, int Column) : IComparable<Point?>
    {
        public int CompareTo(Point? other) =>
            other is null
                ? -1
                : (Row, Column).CompareTo((other.Row, other.Column));

        public static Point operator +(Point left, Point right)
            => new(left.Row + right.Row, left.Column + right.Column);

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

        public static implicit operator Point((int X, int Y) i) => new(i.X, i.Y);
    }
}
