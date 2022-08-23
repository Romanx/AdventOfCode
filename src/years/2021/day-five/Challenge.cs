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

record GridLine
{
    private GridLine(Point2d start, Point2d end, LineType type)
    {
        Start = start;
        End = end;
        Type = type;
    }

    public IEnumerable<Point2d> Points => CalculatePoints();

    public Point2d Start { get; }

    public Point2d End { get; }

    public LineType Type { get; }

    private IEnumerable<Point2d> CalculatePoints()
    {
        var x = (End.X - Start.X) switch
        {
            0 => 0,
            > 0 => 1,
            < 0 => -1,
        };

        var y = (End.Y - Start.Y) switch
        {
            0 => 0,
            > 0 => 1,
            < 0 => -1,
        };
        var adjustment = new Point2d(x, y);

        var start = Start;
        while (start != End)
        {
            yield return start;
            start += adjustment;
        }
        yield return End;
    }

    public static GridLine Create(Point2d start, Point2d end)
    {
        if (start.Y == end.Y)
        {
            return new GridLine(start, end, LineType.Horizontal);
        }
        else if (start.X == end.X)
        {
            return new GridLine(start, end, LineType.Vertical);
        }

        var slope = PointHelpers.SlopeBetweenTwoPoints(start, end);
        if (slope is 1 or -1)
        {
            return new GridLine(start, end, LineType.Diagonal);
        }

        throw new InvalidOperationException("There isn't a straight line between the points!");
    }
}

enum LineType
{
    Unknown,
    Horizontal,
    Vertical,
    Diagonal,
}


internal static class ParseExtensions
{
    public static ImmutableArray<GridLine> ParseVents(this IInputLines lines)
    {
        var builder = ImmutableArray.CreateBuilder<GridLine>();
        foreach (var line in lines.AsString())
        {
            builder.Add(ParseLine(line));
        }

        return builder.ToImmutable();

        static GridLine ParseLine(string line)
        {
            var split = line.Split("->", StringSplitOptions.TrimEntries);

            var start = Point2d.Parse(split[0]);
            var end = Point2d.Parse(split[1]);

            return GridLine.Create(start, end);
        }
    }
}
