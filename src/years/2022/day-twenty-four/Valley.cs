using System.Collections.Frozen;
using System.Collections.Generic;
using CommunityToolkit.HighPerformance;

namespace DayTwentyFour2022;

readonly record struct PathAttempt(uint Steps, Point2d Location)
{
    public PathAttempt Move(Point2d next) => new(Steps + 1, next);

    public PathAttempt Move() => new(Steps + 1, Location);
}

readonly record struct Valley(
    ReadOnlyMemory2D<char> Map,
    FrozenSet<Point2d> Taken,
    Blizzard[] Blizzards) : IEquatable<Valley>
{
    public bool IsOpen(Point2d position)
        => Taken.Contains(position) is false;

    public bool InBounds(Point2d position)
        => Map.TryGetAt(position, out var at) is true && at is not '#';

    public Valley Next()
    {
        var (blizzards, taken) = StepBlizzards(Blizzards, Map);

        return this with
        {
            Blizzards = blizzards,
            Taken = taken,
        };

        static (Blizzard[], FrozenSet<Point2d>) StepBlizzards(Blizzard[] blizzards, ReadOnlyMemory2D<char> map)
        {
            var set = new HashSet<Point2d>(blizzards.Length);
            var next = new Blizzard[blizzards.Length];
            for (var i = 0; i < blizzards.Length; i++)
            {
                var moved = blizzards[i].Move(map.Span);
                set.Add(moved.Position);
                next[i] = moved;
            }

            return (next, set.ToFrozenSet());
        }
    }

    public string Print(Point2d current)
    {
        var grid = new char[Map.Height, Map.Width];
        Map.CopyTo(grid);

        grid[current.Y, current.X] = 'E';

        foreach (var blizzard in Blizzards)
        {
            var (x, y) = blizzard.Position;
            ref var at = ref grid[y, x];

            // If at is empty then set the value
            if (at is '.')
            {
                SetPosition(ref at, blizzard);
            }
            // If the at is a number then increment it.
            else if (char.IsAsciiDigit(at))
            {
                var next = (at - '0') + 1;
                at = (char)('0' + next);
            }
            // Otherwise we must have a blizzard already there so set it to 2.
            else
            {
                at = '2';
            }
        }

        return GridPrinter.Print(grid);

        static void SetPosition(ref char position, Blizzard blizzard)
        {
            position = blizzard.Facing.DirectionType switch
            {
                DirectionType.North => '^',
                DirectionType.East => '>',
                DirectionType.South => 'v',
                DirectionType.West => '<',
                _ => throw new NotImplementedException(),
            };
        }
    }

    public override int GetHashCode()
    {
        HashCode hc = default;
        foreach (var blizzard in Blizzards)
        {
            hc.Add(blizzard);
        }

        return hc.ToHashCode();
    }

    public bool Equals(Valley other) =>
        Blizzards.AsSpan().SequenceEqual(other.Blizzards);
}
