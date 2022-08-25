using Shared.Grid;

namespace DayFive2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 5), "Hydrothermal Venture");

    public void PartOne(IInput input, IOutput output)
    {
        var numberOfOverlappingVents = input.Lines.ParseVents()
            .Where(v => v.Type is LineType.Horizontal or LineType.Vertical)
            .SelectMany(i => i.Points)
            .GroupBy(i => i)
            .Where(i => i.Count() >= 2)
            .Select(i => i.Key)
            .Count();

        output.WriteProperty("Number of Overlapping Vents", numberOfOverlappingVents);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var numberOfOverlappingVents = input.Lines.ParseVents()
            .SelectMany(i => i.Points)
            .GroupBy(i => i)
            .Where(i => i.Count() >= 2)
            .Select(i => i.Key)
            .Count();

        output.WriteProperty("Number of Overlapping Vents", numberOfOverlappingVents);
    }
}


internal static class ParseExtensions
{
    public static ImmutableArray<LineSegment> ParseVents(this IInputLines lines)
    {
        var builder = ImmutableArray.CreateBuilder<LineSegment>();
        foreach (var line in lines.AsString())
        {
            builder.Add(ParseLine(line));
        }

        return builder.ToImmutable();

        static LineSegment ParseLine(string line)
        {
            var split = line.Split("->", StringSplitOptions.TrimEntries);

            var start = Point2d.Parse(split[0]);
            var end = Point2d.Parse(split[1]);

            return LineSegment.Create(start, end);
        }
    }
}
