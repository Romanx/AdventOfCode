using Shared.Grid;
using Spectre.Console;

namespace DayNine2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 9), "Rope Bridge");

    public void PartOne(IInput input, IOutput output)
    {
        var vectors = input.Parse();
        var tail = new HashSet<Point2d>()
        {
            Point2d.Origin,
        };

        var knots = new Point2d[2];

        foreach (var vector in vectors)
        {
            knots = ApplyVector(vector, knots, tail);
        }

        output.WriteProperty("Tail visited count", tail.Count);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var vectors = input.Parse();
        var tail = new HashSet<Point2d>()
        {
            Point2d.Origin,
        };

        var knots = new Point2d[10];

        foreach (var vector in vectors)
        {
            knots = ApplyVector(vector, knots, tail);
        }

        output.WriteProperty("Tail visited count", tail.Count);
    }

    private static Point2d[] ApplyVector(
        Vector vector,
        Point2d[] knots,
        ISet<Point2d> tail)
    {
        var next = new Point2d[knots.Length];
        knots.CopyTo(next.AsSpan());

        for (var i = 0; i < vector.Magnitude; i++)
        {
            var head = next[0];
            var newHead = head + vector.Direction;
            next[0] = newHead;

            for (var x = 1; x < knots.Length; x++)
            {
                var previous = next[x - 1];
                var current = next[x];

                // If the tail is more than one away move it to where the head was
                next[x] = Point2d.Distance(previous, current) > 1
                    ? current + Point2d.DirectionBetweenPoints(current, previous)
                    : current;
            }

            tail.Add(next[^1]);
        }

        return next;
    }

    static string PrintKnots(Point2d[] knots)
    {
        var area = Area2d.Create(knots);
        area = area.Pad(1);

        var dict = area.Items.ToDictionary(k => k, v => '.');

        for (var i = knots.Length - 1; i >= 0; i--)
        {
            var point = knots[i];
            dict[point] = i is 0
                ? 'H'
                : (char)('0' + i);
        }

        return GridPrinter.Print(dict);
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Vector> Parse(this IInput input)
    {
        var builder = ImmutableArray.CreateBuilder<Vector>(input.Lines.Length);

        foreach (var line in input.Lines.AsMemory())
        {
            var span = line.Span;

            var direction = GridDirection.FromChar(span[0]);
            var magnitude = int.Parse(span[1..]);

            builder.Add(new Vector(direction, magnitude));
        }

        return builder.MoveToImmutable();
    }
}

/// <summary>
/// A vector representing a direction and magnitude
/// </summary>
/// <param name="Direction"></param>
/// <param name="Magnitude"></param>
readonly record struct Vector(Direction Direction, int Magnitude);
