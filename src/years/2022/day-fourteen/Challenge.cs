using MoreLinq;
using Shared.Grid;
using SixLabors.ImageSharp;

namespace DayFourteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 14), "Regolith Reservoir");

    public void PartOne(IInput input, IOutput output)
    {
        var rocks = input.Parse();
        var simulator = new Simulator(rocks);

        var sand = simulator.SimulateSand(includeFloor: false);

        output.WriteProperty("Resting Sand", sand.Count);
        output.AddImage("part-1", PrintScan(rocks, sand));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var rocks = input.Parse();
        var simulator = new Simulator(rocks);
        var sand = simulator.SimulateSand(includeFloor: true);

        output.WriteProperty("Resting Sand", sand.Count);
        output.AddImage("part-2", PrintScan(rocks, sand));
    }

    private static Image PrintScan(
        ImmutableHashSet<Point2d> rocks,
        ImmutableHashSet<Point2d> sand)
    {
        var dict = ImmutableDictionary.CreateBuilder<Point2d, Color>();

        dict.AddRange(rocks.Select(r => KeyValuePair.Create(r, Color.DarkSlateGray)));
        dict.AddRange(sand.Select(s => KeyValuePair.Create(s, Color.LightGoldenrodYellow)));

        return new GridImageWriter(dict.ToImmutable()).Generate(10);
    }
}

public class Simulator
{
    private readonly ImmutableHashSet<Point2d> rocks;
    private readonly Point2d sandOrigin;
    private readonly Area2d area;
    private readonly int floor;

    public Simulator(ImmutableHashSet<Point2d> rocks)
    {
        this.rocks = rocks;
        sandOrigin = new Point2d(500, 0);
        area = Area2d.Create(rocks.Add(sandOrigin));
        floor = area.YRange.Max + 2;
    }

    public ImmutableHashSet<Point2d> SimulateSand(bool includeFloor)
    {
        var all = ImmutableHashSet.CreateBuilder<Point2d>();
        all.UnionWith(rocks);
        var sand = ImmutableHashSet.CreateBuilder<Point2d>();

        while (true)
        {
            Func<Point2d, ISet<Point2d>, bool> blockedCheck = includeFloor
                ? IsBlockedByOtherOrFloor
                : IsBlockedByOther;

            Func<Point2d, bool> cantSettleCheck = includeFloor
                ? current => false
                : current => area.Contains(current) is false;

            var settled = DropUntilSettled(
                all,
                cantSettleCheck,
                blockedCheck);

            if (settled.HasValue is false || settled.Value == sandOrigin)
            {
                if (settled.HasValue)
                {
                    sand.Add(settled.Value);
                    all.Add(settled.Value);
                }

                return sand.ToImmutable();
            }

            sand.Add(settled.Value);
            all.Add(settled.Value);
        }

        static bool IsBlockedByOther(Point2d current, ISet<Point2d> all)
        {
            return all.Contains(current);
        }

        bool IsBlockedByOtherOrFloor(Point2d current, ISet<Point2d> all)
        {
            if (current.Y == floor)
            {
                return true;
            }

            return all.Contains(current);
        }
    }

    private Point2d? DropUntilSettled(
        ISet<Point2d> all,
        Func<Point2d, bool> cantSettleCheck,
        Func<Point2d, ISet<Point2d>, bool> isBlocked)
    {
        var current = sandOrigin;

        while (true)
        {
            // If we fell off the end then we can't settle
            if (cantSettleCheck(current))
            {
                return null;
            }

            var down = current + Direction.South;
            var right = current + Direction.SouthEast;
            var left = current + Direction.SouthWest;

            if (isBlocked(down, all) is false)
            {
                current = down;
            }
            else if (isBlocked(left, all) is false)
            {
                current = left;
            }
            else if (isBlocked(right, all) is false)
            {
                current = right;
            }
            else
            {
                return current;
            }
        }
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"([-+]?\d+,\s*[-+]?\d+)");

    public static ImmutableHashSet<Point2d> Parse(this IInput input)
    {
        var builder = ImmutableHashSet.CreateBuilder<Point2d>();

        foreach (var line in input.Lines.AsMemory())
        {
            var span = line.Span;
            var matches = regex.Matches(span)
                .ToList(match =>
                {
                    return Point2d.Parse(match.Groups[1].Value);
                });

            var points = matches.Window(2)
                .SelectMany(window => LineSegment.Create(window[0], window[1]).Points);

            builder.UnionWith(points);
        }

        return builder.ToImmutableHashSet();
    }
}
