namespace DayThirteen2018;

class Minecart : IComparable<Minecart>
{
    public Point2d Position { get; private set; }
    public Direction Facing { get; private set; }
    public byte IntersectionCount { get; private set; }
    public bool Alive { get; set; } = true;

    public Minecart(Point2d position, Direction facing, byte intersectionCount)
    {
        Position = position;
        Facing = facing;
        IntersectionCount = intersectionCount;
    }

    public bool CollidesWith(Minecart other)
        => this != other && this.Alive && other.Alive && Position == other.Position;

    public void Move(TrackSystem track)
    {
        var next = Position + Facing;

        var (direction, intersectionCount) = track[next] switch
        {
            TrackType.None => throw new InvalidOperationException("We shouldn't ever reach none!"),
            TrackType.CurveLeft => Facing.DirectionType switch
            {
                DirectionType.North or DirectionType.South => (Facing.Left(), IntersectionCount),
                _ => (Facing.Right(), IntersectionCount),
            },
            TrackType.CurveRight => Facing.DirectionType switch
            {
                DirectionType.East or DirectionType.West => (Facing.Left(), IntersectionCount),
                _ => (Facing.Right(), IntersectionCount),
            },
            TrackType.Intersection => (IntersectionCount switch
            {
                0 => Facing.Left(),
                1 => Facing,
                2 => Facing.Right(),
                _ => throw new InvalidOperationException("Should have handled this!"),
            }, (IntersectionCount + 1) % 3),
            _ => (Facing, IntersectionCount),
        };

        Position = next;
        Facing = direction;
        IntersectionCount = (byte)intersectionCount;
    }

    public int CompareTo(Minecart? other) => other is not null
        ? ReadingOrderComparer.Instance.Compare(Position, other.Position)
        : 0;

    public static bool operator <(Minecart left, Minecart right) => left.CompareTo(right) < 0;

    public static bool operator >(Minecart left, Minecart right) => left.CompareTo(right) > 0;

    public static bool operator <=(Minecart left, Minecart right) => left.CompareTo(right) <= 0;

    public static bool operator >=(Minecart left, Minecart right) => left.CompareTo(right) >= 0;
}
