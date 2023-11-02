using System;
using System.Diagnostics.CodeAnalysis;

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
        None,
    }

    public readonly record struct Direction :
        IComparable<Direction>,
        IParsable<Direction>,
        ISpanParsable<Direction>
    {
        private Direction(DirectionType directionType)
        {
            DirectionType = directionType;
        }

        public DirectionType DirectionType { get; }

        public static Direction None { get; } = new Direction(DirectionType.None);
        public static Direction North { get; } = new Direction(DirectionType.North);
        public static Direction NorthEast { get; } = new Direction(DirectionType.NorthEast);
        public static Direction NorthWest { get; } = new Direction(DirectionType.NorthWest);
        public static Direction East { get; } = new Direction(DirectionType.East);
        public static Direction South { get; } = new Direction(DirectionType.South);
        public static Direction SouthEast { get; } = new Direction(DirectionType.SouthEast);
        public static Direction SouthWest { get; } = new Direction(DirectionType.SouthWest);
        public static Direction West { get; } = new Direction(DirectionType.West);

        public Direction Right() => DirectionActions.TurnRight(this);

        public Direction Left() => DirectionActions.TurnLeft(this);

        public Direction Reverse() => DirectionActions.Reverse(this);

        public Direction Turn(Direction direction) => direction.DirectionType switch
        {
            DirectionType.East => Right(),
            DirectionType.West => Left(),
            _ => throw new InvalidOperationException($"Unable to turn direction: '{direction}'")
        };

        public int CompareTo(Direction other) => DirectionType.CompareTo(other.DirectionType);

        public static Direction Parse(string s, IFormatProvider? provider = null)
            => Parse(s.AsSpan(), provider);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Direction result)
            => TryParse(s.AsSpan(), provider, out result);

        public static Direction Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
            => new(Enum.Parse<DirectionType>(s, true));

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Direction result)
        {
            if (Enum.TryParse<DirectionType>(s, true, out var directionType))
            {
                result = new(directionType);
                return true;
            }

            result = default;
            return false;
        }

        public override string ToString()
            => $"{DirectionType}";
    }

    public static class DirectionActions
    {
        public static Direction None(Direction direction) => direction;

        public static Direction TurnRight(Direction direction)
            => Directions.CardinalDirections[(Directions.CardinalDirections.IndexOf(direction) + 1) % 4];

        public static Direction TurnLeft(Direction direction)
            => Directions.CardinalDirections[(Directions.CardinalDirections.IndexOf(direction) + 3) % 4];

        public static Direction Reverse(Direction direction) => Directions.All[(Directions.All.IndexOf(direction) + 4) % Directions.All.Length];
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
