namespace DayTwentyThree2022;

public static class AdjacentPointsExtensions
{
    public static bool Any(this AdjacentPoints2d adjacent, Direction direction) => direction.DirectionType switch
    {
        DirectionType.North => AnyNorth(adjacent),
        DirectionType.East => AnyEast(adjacent),
        DirectionType.South => AnySouth(adjacent),
        DirectionType.West => AnyWest(adjacent),
    };

    public static bool AnyNorth(this AdjacentPoints2d adjacent)
        => adjacent[Direction.NorthWest] is not null ||
           adjacent[Direction.North] is not null ||
           adjacent[Direction.NorthEast] is not null;

    public static bool AnyEast(this AdjacentPoints2d adjacent)
        => adjacent[Direction.NorthEast] is not null ||
           adjacent[Direction.East] is not null ||
           adjacent[Direction.SouthEast] is not null;

    public static bool AnySouth(this AdjacentPoints2d adjacent)
        => adjacent[Direction.SouthEast] is not null ||
           adjacent[Direction.South] is not null ||
           adjacent[Direction.SouthWest] is not null;

    public static bool AnyWest(this AdjacentPoints2d adjacent)
        => adjacent[Direction.NorthWest] is not null ||
           adjacent[Direction.West] is not null ||
           adjacent[Direction.SouthWest] is not null;

}

