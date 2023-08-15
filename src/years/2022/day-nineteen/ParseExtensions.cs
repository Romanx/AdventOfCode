using CommunityToolkit.HighPerformance;

namespace DayNineteen2022;

internal static class ParseExtensions
{
    private static readonly PcreRegex blueprintRegex = new(@"Blueprint (?<Id>\d*):");
    private static readonly PcreRegex costRegex = new(@"Each (?<Type>\w*) robot costs (?<Costs>[^.]*).");

    public static ImmutableArray<Blueprint> Parse(this IInput input)
    {
        var lines = input.Lines.AsArray();
        var builder = ImmutableArray.CreateBuilder<Blueprint>(lines.Length);

        foreach (var line in lines)
        {
            builder.Add(ParseBlueprint(line));
        }

        return builder.MoveToImmutable();
    }

    public static Blueprint ParseBlueprint(string line)
    {
        var span = line.AsSpan();
        var match = blueprintRegex.Match(span);

        var number = byte.Parse(match.Groups["Id"].Value, null);
        var robots = ParseRobotBlueprint(span);

        return new Blueprint(number, robots);
    }

    public static ImmutableArray<RobotBlueprint> ParseRobotBlueprint(ReadOnlySpan<char> span)
    {
        var builder = ImmutableArray.CreateBuilder<RobotBlueprint>();
        var matches = costRegex.Matches(span);

        foreach (var match in matches)
        {
            var type = Enum.Parse<Type>(match.Groups["Type"], true);
            var str = new string(match.Groups["Costs"].Value);
            var split = str.Split("and", StringSplitOptions.TrimEntries);

            var resources = ResourceState.CreateBuilder();
            foreach (var item in split)
            {
                ParseResource(ref resources, item);
            }

            builder.Add(new RobotBlueprint(type, resources.Build()));
        }

        return builder.ToImmutable();

        static void ParseResource(ref ResourceState.Builder resources, ReadOnlySpan<char> span)
        {
            var split = span.IndexOf(' ');
            var amount = byte.Parse(span[..split]);
            var type = Enum.Parse<Type>(span[(split + 1)..], true);

            resources[type] = amount;
        }
    }
}
