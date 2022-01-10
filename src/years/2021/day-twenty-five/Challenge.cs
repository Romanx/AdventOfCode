using System.Text;
using Shared.Grid;
using Spectre.Console;

namespace DayTwentyFive2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 25), "Sea Cucumber");

    public override void PartOne(IInput input, IOutput output)
    {
        var map = input.Lines.ParseMap();
        WriteBlock(output, "Original", map);

        var (steps, final) = RunUntilNotMoving(map);

        WriteBlock(output, "Final", final);
        output.WriteProperty("Number of steps", steps);

        static void WriteBlock(IOutput output, string header, Map map)
        {
            output.WriteBlock(() =>
            {
                return new Panel(map.Print())
                {
                    Header = new PanelHeader(header)
                };
            });
        }
    }

    public override void PartTwo(IInput input, IOutput output)
    {
    }

    private static (int Steps, Map Map) RunUntilNotMoving(Map current)
    {
        var seen = new HashSet<Map>()
        {
            current
        };

        for (var steps = 1; ; steps++)
        {
            var next = current.Step();
            if (seen.Add(next) is false)
            {
                return (steps, next);
            }
            current = next;
        }
    }
}

internal static class ParseExtensions
{
    public static Map ParseMap(this IInputLines lines)
    {
        var arr = lines.As2DArray();
        var area = Area2d.Create(arr);

        var cucumbers = ImmutableHashSet.CreateBuilder<SeaCucumber>();
        foreach (var point in area.Items)
        {
            var c = arr[point.Y, point.X];
            if (c is '>' or 'v')
            {
                cucumbers.Add(new SeaCucumber(point, c is '>' ? Direction.East : Direction.South));
            }
        }

        return new Map(cucumbers.ToImmutable(), area);
    }
}

readonly record struct Map(ImmutableHashSet<SeaCucumber> Cucumbers, Area2d Area)
{
    public Map Step()
    {
        var current = Cucumbers;
        var next = Cucumbers.ToBuilder();

        StepDirection(Area, Cucumbers.Where(s => s.Facing.DirectionType is DirectionType.East), current, next);

        current = next.ToImmutable();
        StepDirection(Area, Cucumbers.Where(s => s.Facing.DirectionType is DirectionType.South), current, next);

        return new Map(next.ToImmutable(), Area);

        static void StepDirection(
            Area2d area,
            IEnumerable<SeaCucumber> candidates,
            IReadOnlySet<SeaCucumber> current,
            ImmutableHashSet<SeaCucumber>.Builder next)
        {
            foreach (var cucumber in candidates)
            {
                var target = cucumber.Position + cucumber.Facing;

                // If we're no longer in the area then we will wrap
                if (area.Contains(target) is false)
                {
                    target = cucumber.Facing.DirectionType is DirectionType.East
                        ? new Point2d(area.XRange.Min, target.Y)
                        : new Point2d(target.X, area.YRange.Min);
                }

                var r = cucumber with { Position = target };

                // If there's nothing at the target location
                if (current.Contains(r) is false)
                {
                    next.Add(r);
                    next.Remove(cucumber);
                }
            }
        }
    }

    public string Print()
    {
        var result = new StringBuilder();

        var (xRange, yRange) = Area;

        var map = Cucumbers
            .ToDictionary(k => k.Position, v => v.Facing);

        for (var y = yRange.Min; y <= yRange.Max; y++)
        {
            for (var x = xRange.Min; x <= xRange.Max; x++)
            {
                var point = new Point2d(x, y);
                if (map.TryGetValue(point, out var facing))
                {
                    result.Append(facing.DirectionType switch
                    {
                        DirectionType.East => ">",
                        DirectionType.South => "v",
                        _ => throw new NotImplementedException(),
                    });
                }
                else
                {
                    result.Append('.');
                }
            }
            result.AppendLine();
        }

        return result.ToString().TrimEnd();
    }

    public override int GetHashCode()
    {
        HashCode hashcode = default;
        foreach (var c in Cucumbers)
        {
            hashcode.Add(c);
        }

        return hashcode.ToHashCode();
    }

    public bool Equals(Map other)
        => Cucumbers.SetEquals(other.Cucumbers);
}

readonly record struct SeaCucumber(Point2d Position, Direction Facing)
{
    public bool Equals(SeaCucumber other) => Position.Equals(other.Position);

    public override int GetHashCode() => Position.GetHashCode();
}
