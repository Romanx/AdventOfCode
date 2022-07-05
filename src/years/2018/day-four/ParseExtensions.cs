namespace DayFour2018;

internal static class ParseExtensions
{
    private static readonly PcreRegex lineRegex = new(@"\[(.*)\] (.*)");
    private static readonly PcreRegex guardRegex = new(@"Guard #(\d+) begins shift");

    public static ImmutableArray<GuardShift> Parse(this IInput input)
    {
        var lines = input.Lines
            .AsMemory()
            .OrderBy(line =>
            {
                var match = lineRegex.Match(line.Span);
                var date = DateTime.Parse(match.Groups[1].Value);
                return date;
            })
            .ToArray();

        return SpanHelpers.SplitByCondition(lines, static line => line.Span.IndexOf("Guard") != -1, true)
            .Select(ParseShift)
            .ToImmutableArray();
    }

    private static GuardShift ParseShift(ReadOnlyMemory<ReadOnlyMemory<char>> group)
    {
        var span = group.Span;
        var (startDate, guardLine) = SplitLine(span[0]);
        var guardId = int.Parse(guardRegex.Match(guardLine.Span).Groups[1].Value);
        var shiftDate = startDate.Hour is 23
            ? DateOnly.FromDateTime(startDate.Date).AddDays(1)
            : DateOnly.FromDateTime(startDate.Date);

        var times = ParseSleepTimes(group[1..]);
        return new GuardShift(shiftDate, guardId, times);
    }

    private static ImmutableArray<Range> ParseSleepTimes(ReadOnlyMemory<ReadOnlyMemory<char>> lines)
    {
        if (lines.Length % 2 != 0)
            throw new InvalidOperationException("Should have pairs of events!");

        var builder = ImmutableArray.CreateBuilder<Range>(lines.Length / 2);
        for (var i = 0; i < lines.Length; i += 2)
        {
            var (startDate, _) = SplitLine(lines.Span[i]);
            var (endDate, _) = SplitLine(lines.Span[i + 1]);

            var range = startDate.Minute..endDate.Minute;
            builder.Add(range);
        }

        return builder.ToImmutable();
    }

    private static (DateTime EventDate, ReadOnlyMemory<char> Data) SplitLine(ReadOnlyMemory<char> line)
    {
        var match = lineRegex.Match(line.Span);
        var date = DateTime.Parse(match.Groups[1].Value);
        return (EventDate: date, Data: new string(match.Groups[2].Value).AsMemory());
    }
}
