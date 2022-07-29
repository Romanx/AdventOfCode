namespace DayThirteen2018;

internal static class ParseExtensions
{
    public static TrackSystem Parse(this IInput input)
    {
        var grid = ImmutableDictionary.CreateBuilder<Point2d, TrackType>();
        var carts = ImmutableArray.CreateBuilder<Minecart>();

        foreach (var (point, character) in input.As2DPoints())
        {
            if (character is '^' or '>' or '<' or 'v')
            {
                var (direction, trackType) = character switch
                {
                    '^' => (Direction.North, TrackType.UpDown),
                    '>' => (Direction.East, TrackType.LeftRight),
                    'v' => (Direction.South, TrackType.UpDown),
                    '<' => (Direction.West, TrackType.LeftRight),
                    _ => throw new InvalidOperationException("This can't happen but sure"),
                };

                carts.Add(new Minecart(point, direction, 0));
                grid.Add(point, trackType);
            }
            else
            {
                var type = character switch
                {
                    ' ' => TrackType.None,
                    '|' => TrackType.UpDown,
                    '-' => TrackType.LeftRight,
                    '/' => TrackType.CurveRight,
                    '\\' => TrackType.CurveLeft,
                    '+' => TrackType.Intersection,
                    _ => throw new NotImplementedException(),
                };

                grid.Add(point, type);
            }
        }

        return new TrackSystem(grid.ToImmutable(), carts.ToImmutable());
    }
}
