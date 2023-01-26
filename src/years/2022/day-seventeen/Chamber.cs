using Shared.Grid;

namespace DaySeventeen2022;

internal class Chamber(Pattern pattern)
{
    private static readonly DimensionRange horizontalRange = new(0, 6);
    private static readonly Point2d Down = (0, -1);

    private readonly HashSet<Point2d> cave = new();

    public int Height => cave.Count > 0
        ? cave.Max(point => point.Y)
        : 0;

    public Pattern Pattern { get; } = pattern;

    public int BlockCount = 0;

    public void DropRock(Rock rock)
    {
        var current = rock.MoveToStart(Height);

        while (true)
        {
            var jet = Pattern.GetAndMoveNext();

            // Move the rock horizontally if possible.
            var updated = current + jet;

            // Make the rock the current one if its in bounds.
            if (InBounds(updated, cave))
            {
                current = updated;
            }

            // Drop by one
            updated = current + Down;

            // If the new position is invalid then add the previous to the taken points.
            if (InBounds(updated, cave) is false)
            {
                cave.UnionWith(current.Points);
                BlockCount++;
                return;
            }
            else
            {
                current = updated;
            }
        }

        static bool InBounds(Rock rock, IReadOnlySet<Point2d> cave)
        {
            foreach (var point in rock.Points)
            {
                // If we're not in horizontal range then we're not in bounds
                if (horizontalRange.Contains(point.X) is false)
                {
                    return false;
                }

                if (point.Y <= 0)
                {
                    return false;
                }

                if (cave.Contains(point))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public static Chamber Create(IInput input)
        => new(new Pattern(input.Content.AsMemory()));

    public string Print()
    {
        var area = Area2d.Create(7, Height + 4);
        var array = area.As2dArray<char>();

        foreach (var (point, screen) in CoordinateSystem.ConvertCartesianToScreen(area.Items))
        {
            array[screen.Y, screen.X] = cave.Contains(point)
                ? '#'
                : '.';
        }

        return GridPrinter.Print(array);
    }

    public ImmutableArray<int> BuildNormalisedCeiling()
    {
        var builder = ImmutableArray.CreateBuilder<int>(7);

        var highestPoints = cave.GroupBy(point => point.X)
            .ToDictionary(k => k.Key, v => v.Max(point => point.Y));

        var height = Height;

        for (var i = 0; i < 7; i++)
        {
            builder.Insert(i, height - highestPoints.GetValueOrDefault(i, 0));
        }

        return builder.MoveToImmutable();
    }
}
