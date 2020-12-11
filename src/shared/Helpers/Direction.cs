using System;
using System.Collections.Immutable;

namespace Shared
{
    public enum DirectionType
    {
        North,
        NorthEast,
        NorthWest,
        South,
        SouthEast,
        SouthWest,
        East,
        West
    }

    public record Direction(DirectionType DirectionType)
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

        private static readonly Direction[] values = new[]
        {
            North,
            East,
            South,
            West
        };

        public Direction Right() => values[(Array.IndexOf(values, this) + 1) % 4];

        public Direction Left() => values[(Array.IndexOf(values, this) + 3) % 4];
    }
}
