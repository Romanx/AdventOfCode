using System.Collections.Immutable;

namespace Shared
{
    public static class Directions
    {
        public static ImmutableArray<Direction> All { get; } =
        [
            Direction.North,
            Direction.NorthEast,
            Direction.NorthWest,
            Direction.East,
            Direction.South,
            Direction.SouthEast,
            Direction.SouthWest,
            Direction.West,
        ];

        public static ImmutableArray<Direction> CardinalDirections { get; } = [Direction.North, Direction.East, Direction.South, Direction.West];
    }
}
