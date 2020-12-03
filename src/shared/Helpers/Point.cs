namespace Shared
{
    public record Point(int X, int Y)
    {
        public static Point operator +(Point left, Point right)
            => new(left.X + right.X, left.Y + right.Y);

        public static Point operator +(Point point, Direction direction) => direction.DirectionType switch
        {
            DirectionType.North => point + (0, 1),
            DirectionType.East => point + (1, 0),
            DirectionType.South => point + (0, -1),
            DirectionType.West => point + (-1, 0),
            _ => point
        };

        public static implicit operator Point((int X, int Y) i) => new(i.X, i.Y);
    }
}
