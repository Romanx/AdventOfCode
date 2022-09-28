using System;
using System.Collections.Immutable;

namespace Shared
{
    public enum DirectionType
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
    }

    public record class Direction : IComparable<Direction?>, IEquatable<Direction>
    {
        private Direction(DirectionType directionType)
        {
            DirectionType = directionType;
        }

        public DirectionType DirectionType { get; }

        public static Direction North { get; } = new Direction(DirectionType.North);
        public static Direction NorthEast { get; } = new Direction(DirectionType.NorthEast);
        public static Direction NorthWest { get; } = new Direction(DirectionType.NorthWest);
        public static Direction East { get; } = new Direction(DirectionType.East);
        public static Direction South { get; } = new Direction(DirectionType.South);
        public static Direction SouthEast { get; } = new Direction(DirectionType.SouthEast);
        public static Direction SouthWest { get; } = new Direction(DirectionType.SouthWest);
        public static Direction West { get; } = new Direction(DirectionType.West);

        public static ImmutableArray<Direction> All { get; } = ImmutableArray.Create(
            North, NorthEast, NorthWest, East, South, SouthEast, SouthWest, West);

        public static ImmutableArray<Direction> CardinalDirections { get; } = ImmutableArray.Create(
            North, East, South, West);

        public static Direction Parse(string value) => new(Enum.Parse<DirectionType>(value, true));

        public Direction Right() => DirectionActions.TurnRight(this);

        public Direction Left() => DirectionActions.TurnLeft(this);

        public Direction Reverse() => DirectionActions.Reverse(this);

        public int CompareTo(Direction? other) => DirectionType.CompareTo(other?.DirectionType);

        public override int GetHashCode()
        {
            return HashCode.Combine(DirectionType);
        }
    }

    public static class DirectionActions
    {
        public static Direction None(Direction direction) => direction;

        public static Direction TurnRight(Direction direction)
            => Direction.CardinalDirections[(Direction.CardinalDirections.IndexOf(direction) + 1) % 4];

        public static Direction TurnLeft(Direction direction)
            => Direction.CardinalDirections[(Direction.CardinalDirections.IndexOf(direction) + 3) % 4];

        public static Direction Reverse(Direction direction) => Direction.All[(Direction.All.IndexOf(direction) + 4) % Direction.All.Length];
    }

    public static class GridDirection
    {
        public static Direction Up { get; } = Direction.North;
        public static Direction Down { get; } = Direction.South;
        public static Direction Left { get; } = Direction.West;
        public static Direction Right { get; } = Direction.East;

        public static Direction FromChar(char identifier)
        {
            return identifier switch
            {
                'U' => Up,
                'D' => Down,
                'L' => Left,
                'R' => Right,
                _ => throw new InvalidOperationException(),
            };
        }

        public static char ToChar(Direction direction)
        {
            return direction switch
            {
                { DirectionType: DirectionType.North } => 'U',
                { DirectionType: DirectionType.South } => 'D',
                { DirectionType: DirectionType.West } => 'L',
                { DirectionType: DirectionType.East } => 'R',
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
