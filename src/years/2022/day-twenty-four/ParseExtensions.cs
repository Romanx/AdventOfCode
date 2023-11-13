using System.Collections.Frozen;
using CommunityToolkit.HighPerformance;

namespace DayTwentyFour2022;

internal static class ParseExtensions
{
    public static (Valley Valley, Point2d Start, Point2d Exit) ParseValley(this IInput input)
    {
        var map = input.Lines
            .As2DArray()
            .AsMemory2D();

        Memory2D<char> grid = new char[map.Height, map.Width];
        map.CopyTo(grid);
        var span = grid.Span;

        var start = span.GetRowSpan(0).IndexOf('.');
        var exit = span.GetRowSpan(span.Height - 1).IndexOf('.');

        var blizzards = ParseBlizzard(span);
        var taken = blizzards.Select(b => b.Position).ToFrozenSet();

        var valley = new Valley(grid, taken, blizzards);

        return (valley, (start, 0), (exit, span.Height - 1));
    }

    static Blizzard[] ParseBlizzard(Span2D<char> map)
    {
        var result = new List<Blizzard>();

        var minX = 1;
        var maxX = map.Width - 2;
        var minY = 1;
        var maxY = map.Height - 2;

        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                if (map[y, x] is '>' or '<' or '^' or 'v')
                {
                    var value = map[y, x];
                    var current = (x, y);
                    var blizzard = value switch
                    {
                        '^' => new Blizzard(
                            current,
                            (x, minY),
                            (x, maxY),
                            Direction.North),
                        'v' => new Blizzard(
                            current,
                            (x, minY),
                            (x, maxY),
                            Direction.South),
                        '<' => new Blizzard(
                            current,
                            (minX, y),
                            (maxX, y),
                            Direction.West),
                        '>' => new Blizzard(
                            current,
                            (minX, y),
                            (maxX, y),
                            Direction.East),
                        _ => throw new NotImplementedException(),
                    };
                    result.Add(blizzard);
                    map[y, x] = '.';
                }
            }
        }

        return [.. result];
    }
}
