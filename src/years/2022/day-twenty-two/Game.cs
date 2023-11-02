using System.Collections.Frozen;
using CommunityToolkit.HighPerformance;
using Shared.Grid;

namespace DayTwentyTwo2022;

class RegionalGame(RegionPlayer player)
{
    public RegionPlayer Player { get; private set; } = player;

    internal void Apply(Command command)
    {
        if (command is MoveCommand mc)
        {
            for (var i = 0; i < mc.Number; i++)
            {
                var moved = Player.TryMove(out var next);
                if (moved is false)
                {
                    return;
                }
                Player = next;
            }
        }
        else if (command is TurnCommand turn)
        {
            Player = Player.Turn(turn.Direction);
        }
    }

    internal string Score()
    {
        var (position, facing) = Player.Orientation;
        var absolute = Player.Region.ToAbsolute(position);

        var facingValue = facing.DirectionType switch
        {
            DirectionType.East => 0,
            DirectionType.South => 1,
            DirectionType.West => 2,
            DirectionType.North => 3,
        };

        var adjustX = absolute.X + 1;
        var adjustY = absolute.Y + 1;

        return $"{(1000 * adjustY) + (4 * adjustX) + facingValue}";
    }
}

readonly record struct RegionPlayer(
    Orientation Orientation,
    Region Region)
{
    public bool TryMove(out RegionPlayer player)
        => Region.TryMove(Orientation, out player);

    public RegionPlayer Turn(Direction direction)
        => this with
        {
            Orientation = Orientation with
            {
                Facing = Orientation.Facing.Turn(direction),
            },
        };
}

record Region(
    int Id,
    ReadOnlyMemory2D<char> Map,
    Point2d TopLeft)
{
    private IReadOnlyDictionary<Direction, Transition> transitions = ImmutableDictionary<Direction, Transition>.Empty;

    public Area2d Area { get; } = Area2d.Create(Map.Span);

    public Point2d ToAbsolute(Point2d point)
    {
        if (Area.Contains(point) is false)
        {
            throw new InvalidOperationException("Cannot calculate absolute position by something not in the area.");
        }

        return TopLeft + point;
    }

    internal void AddTransitions(Transition[] transitions)
    {
        var dict = new Dictionary<Direction, Transition>(transitions.Length);
        foreach (var transition in transitions)
        {
            dict[transition.StartFacing] = transition;
        }

        this.transitions = dict.ToFrozenDictionary();
    }

    internal bool TryMove(
        Orientation orientation,
        out RegionPlayer updated)
    {
        var next = new RegionPlayer(orientation.Move(), this);

        if (Area.Contains(next.Orientation.Position) is false)
        {
            var transition = transitions[next.Orientation.Facing];
            next = transition.ApplyTransition(orientation.Position);
        }

        if (next.Region.Map.At(next.Orientation.Position) is '#')
        {
            updated = default;
            return false;
        }

        updated = next;
        return true;
    }
}

record Transition(
    Direction StartFacing,
    Direction EndFacing,
    Region Destination)
{
    public RegionPlayer ApplyTransition(Point2d position)
    {
        var next = (StartFacing.DirectionType, EndFacing.DirectionType) switch
        {
            (DirectionType.North, DirectionType.North) => new Point2d(position.X, Destination.Area.YRange.Max),
            (DirectionType.East, DirectionType.East) => new Point2d(Destination.Area.XRange.Min, position.Y),
            (DirectionType.South, DirectionType.South) => new Point2d(position.X, Destination.Area.YRange.Min),
            (DirectionType.West, DirectionType.West) => new Point2d(Destination.Area.XRange.Max, position.Y),

            (DirectionType.North, DirectionType.East) => new Point2d(Destination.Area.XRange.Min, position.Flip().Y),
            (DirectionType.East, DirectionType.North) => new Point2d(position.Flip().X, Destination.Area.YRange.Max),
            (DirectionType.East, DirectionType.West) => new Point2d(Destination.Area.XRange.Max, Destination.Area.YRange.Max - position.Y),
            (DirectionType.West, DirectionType.East) => new Point2d(Destination.Area.XRange.Min, Destination.Area.YRange.Max - position.Y),
            (DirectionType.West, DirectionType.South) => new Point2d(position.Flip().X, Destination.Area.XRange.Min),
            (DirectionType.South, DirectionType.West) => new Point2d(Destination.Area.XRange.Max, position.Flip().Y),
            _ => throw new NotImplementedException($"Not implemented transition between {StartFacing.DirectionType} and {EndFacing.DirectionType}")
        };

        return new RegionPlayer(
            new Orientation(next, EndFacing),
            Destination);
    }
}

readonly record struct Orientation(Point2d Position, Direction Facing)
{
    public Orientation Move()
        => this with { Position = Position + Facing };
}
