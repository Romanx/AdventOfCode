using Shared.Grid;
using Spectre.Console;

namespace DayTen2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 10), "The Stars Align");

    public void PartOne(IInput input, IOutput output)
    {
        var original = input.Parse();
        var lights = original;

        var current = Area2d.Create(lights.Select(l => l.Position));
        var second = 1;

        while (true)
        {
            lights = lights.Select(l => new Light(l.Position + l.Velocity, l.Velocity)).ToImmutableArray();
            var next = Area2d.Create(lights.Select(l => l.Position));
            if (next.XRange.Size > current.XRange.Size || next.YRange.Size > current.YRange.Size)
            {
                break;
            }

            current = next;
            second++;
        }

        var final = original.Select(light => light.Position + (light.Velocity * (second - 1)));
        var grid = GridPrinter.Print(final, '#');
        output.WriteBlock(() =>
        {
            return new Panel(grid) { Header = new PanelHeader("Final Grid", Justify.Center) };
        });
        output.WriteProperty("Number of seconds", second - 1);
    }

    public void PartTwo(IInput input, IOutput output)
    {
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"position=<(.*)> velocity=<(.*)>");

    public static ImmutableArray<Light> Parse(this IInput input)
    {
        var length = input.Lines.Length;
        var result = ImmutableArray.CreateBuilder<Light>(length);

        foreach (var line in input.Lines.AsMemory())
        {
            var match = regex.Match(line.Span);

            var position = Point2d.Parse(match.Groups[1].Value);
            var velocity = Point2d.Parse(match.Groups[2].Value);

            result.Add(new Light(position, velocity));
        }

        return result.MoveToImmutable();
    }
}

readonly record struct Light(Point2d Position, Point2d Velocity);
