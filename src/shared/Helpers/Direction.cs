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

    public record Direction(DirectionType DirectionType) : IComparable<Direction>
    {
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

        public Direction Right() => CardinalDirections[(CardinalDirections.IndexOf(this) + 1) % 4];

        public Direction Left() => CardinalDirections[(CardinalDirections.IndexOf(this) + 3) % 4];

        public Direction Reverse() => All[(All.IndexOf(this) + 4) % All.Length];

        public int CompareTo(Direction? other) => DirectionType.CompareTo(other?.DirectionType);
    }
}
