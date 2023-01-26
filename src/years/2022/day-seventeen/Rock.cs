using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.HighPerformance;
using MoreLinq.Extensions;
using Spectre.Console;

namespace DaySeventeen2022;

internal readonly record struct Rock(IReadOnlySet<Point2d> Points) : IParsable<Rock>
{
    public static Rock Parse(string s, IFormatProvider? provider = null)
    {
        if (TryParse(s, provider, out var rock))
        {
            return rock;
        }

        throw new InvalidOperationException("Unable to parse Rock");
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider, [MaybeNullWhen(false)]
        out Rock result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        var arr = s.As2DArray();
        var span = arr.AsSpan2D();
        var points = new List<Point2d>(arr.Length);

        for (var y = 0; y < span.Height; y++)
        {
            for (var x = 0; x < span.Width; x++)
            {
                var c = span[y, x];
                if (c is '#')
                {
                    // This is -y since y is ascending
                    points.Add((x, y));
                }
            }
        }

        var set = CoordinateSystem
            .ConvertScreenToCartesian(points)
            .ToImmutableHashSet();

        result = new Rock(set);
        return true;
    }

    public static Rock operator +(Rock rock, Point2d point) => rock.Apply(point);

    public static Rock operator +(Rock rock, char action) => rock.Apply(action);

    public Rock Apply(char pattern)
    {
        return pattern switch
        {
            '<' => Apply((-1, 0)),
            '>' => Apply((1, 0)),
            _ => throw new InvalidOperationException($"Unable to apply Pattern '{pattern}'")
        };
    }

    public Rock Apply(Point2d offset) => this with
    {
        Points = Points.Select(p => p + offset).ToImmutableHashSet(),
    };

    public override string ToString() => string.Join(", ", Points.OrderBy(point => point.X).ThenBy(point => point.Y));

    public Rock MoveToStart(int ceilingHeight)
    {
        return Apply((2, ceilingHeight + 4));
    }

    public bool Overlaps(HashSet<Point2d> cave)
        => Points.Overlaps(cave);
}
